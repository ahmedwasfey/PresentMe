package org.allenai.scienceparse;

import com.gs.collections.api.tuple.Pair;
import com.gs.collections.impl.tuple.Tuples;
import lombok.extern.slf4j.Slf4j;
import lombok.val;
import org.allenai.scienceparse.pdfapi.PDFDoc;
import org.allenai.scienceparse.pdfapi.PDFExtractor;
import org.testng.Assert;
import org.testng.annotations.Test;
import scala.Option;
import scala.Some;

import java.io.IOException;
import java.io.InputStream;
import java.sql.Date;
import java.util.Arrays;
import java.util.List;
import java.util.regex.Pattern;
import java.util.stream.Collectors;

@Test
@Slf4j
public class PDFToCRFInputTest {
  public String filePathOfResource(String path) {
    return this.getClass().getResource(path).getFile();
  }

  public void testGetPaperTokens() throws IOException {
    InputStream pdfInputStream = PDFToCRFInputTest.class.getResourceAsStream("/P14-1059.pdf");
    PDFDoc doc = new PDFExtractor().extractFromInputStream(pdfInputStream);
    List<PaperToken> pts = PDFToCRFInput.getSequence(doc);
    log.info("got " + pts.size() + " things.");
    assert (pts.size() > 50);
  }

  public void testFindString() throws IOException {
    String target = "How to make words with vectors: Phrase generation in distributional semantics";
    InputStream pdfInputStream = PDFToCRFInputTest.class.getResourceAsStream("/P14-1059.pdf");
    PDFDoc doc = new PDFExtractor().extractFromInputStream(pdfInputStream);
    List<PaperToken> pts = PDFToCRFInput.getSequence(doc);
    Pair<Integer, Integer> pos = PDFToCRFInput.findString(PDFToCRFInput.asStringList(pts), target);
    Pair<Integer, Integer> posNot = PDFToCRFInput.findString(PDFToCRFInput.asStringList(pts), "this string won't be found");

    Assert.assertTrue(pos != null);
    Assert.assertTrue(pos.getOne() > 0 && (pos.getTwo() - pos.getOne() == 11));
    log.info("found title at " + pos.getOne() + ", " + pos.getTwo());
    log.info("title is " + PDFToCRFInput.stringAt(pts, pos));
    Assert.assertTrue(posNot == null);
  }

  public void testLabelMetadata() throws IOException {
    InputStream pdfInputStream = PDFToCRFInputTest.class.getResourceAsStream("/P14-1059.pdf");
    PDFDoc doc = new PDFExtractor().extractFromInputStream(pdfInputStream);
    List<PaperToken> pts = PDFToCRFInput.getSequence(doc);
    final ExtractedMetadata em = new ExtractedMetadata(
        "How to make words with vectors: Phrase generation in distributional semantics",
        Arrays.asList("Georgiana Dinu", "Marco Baroni"),
        new Date(1388556000000L));
    val labeledData = LabeledData$.MODULE$.fromExtractedMetadata("dummyid", em);
    val result = PDFToCRFInput.labelMetadata("P14-1059", pts, labeledData);
    log.info(PDFToCRFInput.getLabelString(result));
    log.info(pts.stream().map((PaperToken p) -> p.getPdfToken().token).collect(Collectors.toList()).toString());
    Assert.assertEquals(result.get(26).getTwo(), "O");
    Assert.assertEquals(result.get(27).getTwo(), "B_T");
    Assert.assertEquals(result.get(34).getTwo(), "I_T");
    Assert.assertEquals(result.get(37).getTwo(), "E_T");
    Assert.assertEquals(result.get(38).getTwo(), "B_A");
    Assert.assertEquals(result.get(47).getTwo(), "O");
    Assert.assertEquals(result.get(47).getOne(), pts.get(46)); //off by one due to start/stop
    Assert.assertEquals(result.get(0).getTwo(), "<S>");
    Assert.assertEquals(result.get(result.size() - 1).getTwo(), "</S>");
  }

  public void testGetSpans() {
    List<String> ls = Arrays.asList("O", "O", "B_A", "I_A", "E_A");
    val spans = ExtractedMetadata.getSpans(ls);
    Assert.assertEquals(spans.size(), 1);
    Assert.assertEquals(spans.get(0).tag, "A");
    Assert.assertEquals(spans.get(0).loc, Tuples.pair(2, 5));
  }

  public void testAuthorPatterns() {
    List<Pair<Pattern, Boolean>> authOpt = PDFToCRFInput.authorToPatternOptPair("Marco C. Baroni");
    Assert.assertTrue(authOpt.get(0).getOne().matcher("Marco").matches());
    Assert.assertTrue(authOpt.get(1).getOne().matcher("C").matches());
    Assert.assertTrue(authOpt.get(2).getOne().matcher("Baroni").matches());
    Pair<Integer, Integer> span = PDFToCRFInput.findPatternSequence(Arrays.asList("Marco", "C", "Baroni"), authOpt);
    Assert.assertEquals(span, Tuples.pair(0, 3));
    span = PDFToCRFInput.findPatternSequence(Arrays.asList("Marco", "Baroni"), authOpt);
    Assert.assertEquals(span, Tuples.pair(0, 2));
    authOpt = PDFToCRFInput.authorToPatternOptPair("Marco Baroni");
    span = PDFToCRFInput.findPatternSequence(Arrays.asList("M.", "G.", "Baroni"), authOpt);
    Assert.assertEquals(span, Tuples.pair(0, 3));
    span = PDFToCRFInput.findPatternSequence(Arrays.asList("M.", "G.", "B."), authOpt);
    Assert.assertEquals(span, null);
  }

  public void testAuthor() throws IOException {
    InputStream pdfInputStream = PDFToCRFInputTest.class.getResourceAsStream("/P14-1059.pdf");
    PDFDoc doc = new PDFExtractor().extractFromInputStream(pdfInputStream);
    List<PaperToken> pts = PDFToCRFInput.getSequence(doc);
    final ExtractedMetadata em = new ExtractedMetadata(
        "How to make words with vectors: Phrase generation in distributional semantics",
        Arrays.asList("Georgiana Dinu", "Marco C. Baroni"),
        new Date(1388556000000L));
    val labeledData = LabeledData$.MODULE$.fromExtractedMetadata("dummyid", em);

    val result = PDFToCRFInput.labelMetadata("P14-1059", pts, labeledData);
    Assert.assertEquals(result.get(38).getTwo(), "B_A");
    Assert.assertEquals(result.get(39).getTwo(), "E_A");
    Assert.assertEquals(result.get(41).getTwo(), "B_A");
    Assert.assertEquals(result.get(42).getTwo(), "E_A");
  }
}
