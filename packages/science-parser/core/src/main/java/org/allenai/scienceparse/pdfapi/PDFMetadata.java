package org.allenai.scienceparse.pdfapi;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.ObjectWriter;
import lombok.Builder;
import lombok.Data;
import lombok.SneakyThrows;
import lombok.val;

import java.io.FileInputStream;
import java.io.InputStream;
import java.util.Date;
import java.util.List;

/**
 * Immutable class representing information obtained from scanning for PDF
 * meta-data. Many pdf creation programs (like pdflatex) will actuallly output
 * information like these fields which substantially aids downstream extraction.
 */
@Builder
@Data
public class PDFMetadata {
  public final String title;
  public final List<String> authors;
  public final List<String> keywords;
  public final Date createDate;
  public final Date lastModifiedDate;
  public final String creator;

  // HACK(aria42) For external testing purpose
  @SneakyThrows
  public static void main(String[] args) {
    val extractor = new PDFExtractor();
    ObjectWriter ow = new ObjectMapper().writer();
    if (args.length <= 1)
      ow = ow.withDefaultPrettyPrinter();
    for (final String arg : args) {
      String prefix = "";
      if (args.length > 1)
        prefix = arg + "\t";
      try (InputStream pdfInputStream = new FileInputStream(arg)) {
        try {
          PDFMetadata meta = extractor.extractFromInputStream(pdfInputStream).getMeta();
          String json = ow.writeValueAsString(meta);
          System.out.println(prefix + json);
        } catch (final Exception e) {
          System.out.println(prefix + "ERROR: " + e);
        }
      }
    }
  }
}
