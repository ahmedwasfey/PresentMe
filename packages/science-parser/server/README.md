# Science Parse Server

This is a wrapper that makes the [SP library](../core/README.md) available as a web service. We have a version running at http://scienceparse.allenai.org, so you can try it yourself: http://scienceparse.allenai.org/v1/498bb0efad6ec15dd09d941fb309aa18d6df9f5f

This will show a large amount of JSON. Most of it is body text. You can get a slightly more compact output by skipping the body text: http://scienceparse.allenai.org/v1/498bb0efad6ec15dd09d941fb309aa18d6df9f5f?skipFields=sections

Both of these examples parse the paper with the S2 paper id `498bb0efad6ec15dd09d941fb309aa18d6df9f5f`. You can see that paper here: https://pdfs.semanticscholar.org/498b/b0efad6ec15dd09d941fb309aa18d6df9f5f.pdf

## Parsing your own PDF

If you want to upload your own PDF, you can do that with a HTTP POST:
```
curl -v -H "Content-type: application/pdf" --data-binary @paper.pdf "http://scienceparse.allenai.org/v1"
```

Note that the content type needs to be `application/pdf`, and the URL needs to not have a trailing slash.

## Running the server yourself

You can compile the server into a super-jar with sbt with `sbt server/assembly`. That will download all dependencies, compile them, and build an executable jar with all dependencies bundled. Then, you can start up the server with `java -Xmx6g -jar jarfile.jar`. On first startup, it will download several gigabytes of model files, and then bind to port 8080 on the machine you run it on.

The server takes a few command line arguments. Run it with `java -jar jarfile.jar --help` to see what they are.

Science Parse takes quite a bit of memory, so we recommend running it with `-Xmx6g`. Some documents might require more than that. Science Parse also uses off-heap memory (i.e., memory that's not specified by `-Xmx`), so we recommend that you have at least 2GB free in addition to the heap memory specified with `-Xmx`.

## Feedback mechanism

The server supports something called the "Feedback mechanism". This is a fairly basic way to gather corrections to the extractions SP makes, so we can improve the models. The mechanism is disabled by default, so you shouldn't have to worry about it most of the time.

We don't support this mechanism publically, but if you want to play with it, it should be easy to point it at a postgres database of your choice, and start gathering feedback.
