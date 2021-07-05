package org.allenai.scienceparse

import java.io.{ FileInputStream, File }

import com.gs.collections.impl.set.mutable.UnifiedSet
import org.allenai.common.{ Resource, Logging }
import org.allenai.datastore.Datastores
import org.allenai.scienceparse.Parser.ParseOpts
import scopt.OptionParser

import scala.io.Source
import scala.collection.JavaConverters._

object Training extends App with Datastores with Logging {
  // The Files are all Option[File] defaulting to None. Properly, they should be set to the
  // defaults from the datastore, but if we do that here, they will download several gigabytes
  // of files during startup, even if they are unused later.
  case class Config(
    output: File = null,
    maxHeaderWords: Int = Parser.MAXHEADERWORDS,
    maxIterations: Int = 1000,
    backgroundSampleDocs: Int = 40000,
    backgroundDirectory: Option[File] = None,
    gazetteerFile: Option[File] = None,
    trainFraction: Double = 0.9,
    minYear: Int = 2008,
    maxPaperCount: Int = 34000,
    excludeIdsFile: Option[File] = None,
    minExpectedFeatureCount: Int = 13,
    trainingData: Iterator[LabeledPaper] = LabeledPapersFromDBLP.get
  )

  val parser = new OptionParser[Config](this.getClass.getSimpleName) {
    head("Options that are not specified default to the settings that were used to make the production model.")

    opt[File]('o', "output") required () action { (o, c) =>
      c.copy(output = o)
    } text "The output file"

    opt[Int]("maxHeaderWords") action { (m, c) =>
      c.copy(maxHeaderWords = m)
    } text "Specifies the maximum number of words to use for the header if we don't have any other information about where the header ends"

    opt[Int]("maxIterations") action { (i, c) =>
      c.copy(maxIterations = i)
    } text "Maximum number of iterations during training"

    opt[Int]("backgroundSampleDocs") action { (d, c) =>
      c.copy(backgroundSampleDocs = d)
    } text "The number of documents to use to build the background language model"

    opt[File]("backgroundDirectory") action { (d, c) =>
      c.copy(backgroundDirectory = Some(d))
    } text "The directory in which the background documents are found"

    opt[File]('g', "gazetteerFile") action { (f, c) =>
      c.copy(gazetteerFile = Some(f))
    } text "The gazetteer file"

    opt[Double]("trainFraction") action { (f, c) =>
      c.copy(trainFraction = f)
    } text "The fraction of the ground truth to use for training"

    opt[Int]("minYear") action { (y, c) =>
      c.copy(minYear = y)
    } text "The earliest published year we're willing to consider"

    opt[Int]('c', "maxPaperCount") action { (p, c) =>
      c.copy(maxPaperCount = p)
    } text "The maximum number of labeled documents to consider"

    opt[File]("excludeIdsFile") action { (e, c) =>
      c.copy(excludeIdsFile = Some(e))
    } text "A file with paper IDs to exclude, one per line. We always exclude the papers from the evaluation set."

    opt[Int]("minExpectedFeatureCount") action { (n, c) =>
      c.copy(minExpectedFeatureCount = n)
    } text "The minimum number of times we should see a feature before accepting it."

    opt[Unit]("trainOnDBLP") action { (_, c) =>
      c.copy(trainingData = LabeledPapersFromDBLP.get)
    } text "Train with data from DBLP"

    opt[Unit]("trainOnPMC") action { (_, c) =>
      c.copy(trainingData = LabeledPapersFromPMC.getCleaned.drop(10000))
      // Drop 10000 because we test on those.
    } text "Train with data from PMC"

    opt[Unit]("trainOnBoth") action { (_, c) =>
      c.copy(trainingData = new InterleavingIterator(LabeledPapersFromPMC.getCleaned, LabeledPapersFromDBLP.get))
    } text "Train with data from DBLP and PMC"

    help("help") text "Prints help text"
  }

  parser.parse(args, Config()).foreach { config =>
    val opts = new ParseOpts
    opts.modelFile = config.output.toString
    opts.headerMax = config.maxHeaderWords
    opts.iterations = config.maxIterations
    opts.threads = Runtime.getRuntime.availableProcessors() * 2
    opts.backgroundSamples = config.backgroundSampleDocs

    val backgroundDirectory =
      config.backgroundDirectory.getOrElse(publicDirectory("productionBackgroundDocs", 1).toFile)
    opts.backgroundDirectory = backgroundDirectory.toString

    val gazetteerFile = config.gazetteerFile.getOrElse(Parser.getDefaultGazetteer.toFile)
    opts.gazetteerFile = gazetteerFile.toString

    opts.trainFraction = config.trainFraction
    opts.checkAuthors = true
    opts.minYear = config.minYear
    opts.documentCount = config.maxPaperCount
    opts.minExpectedFeatureCount = config.minExpectedFeatureCount

    val excludedIds = Evaluation.goldDocIds ++ config.excludeIdsFile.map { excludedIdsFile =>
      Resource.using(Source.fromFile(excludedIdsFile)) { source =>
        source.getLines().map(_.trim)
      }.toSet
    }.getOrElse(Set.empty)

    val labeledData = config.trainingData.asJava

    Parser.trainParser(
      labeledData,
      opts,
      UnifiedSet.newSet(excludedIds.toIterable.asJava)
    )

    logger.info(s"New model at ${opts.modelFile}")
  }
}
