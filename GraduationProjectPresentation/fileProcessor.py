from image_placer import imgPlacer
from pdfparser import Parser
import json
from pprint import pprint
from summarizer import Summarizer
import sys
import os


class process:
    @staticmethod
    def process(paperPath):
        fp = fileProcessor()
        fp.process(paperPath)


class fileProcessor:
    def __init__(self):
        self.textParser = Parser('text')
        self.imageParser = Parser('image')

    def process(self, paperPath):
        output = self.all(paperPath)
        return output

    def all(self, paperPath, verbose=False):

        if (verbose): print("Text Parser Starting..")
        textOutPath = self.textParser.process(paperPath)
        if (verbose): print("Text Out Path: ", textOutPath)
        if (verbose): print("Text Parser Finished..")

        if (verbose): print("Image Parser Starting..")
        figOutPath = self.imageParser.process(paperPath)
        if (verbose): print("Image Parser Finished..")

        mySummarizer = Summarizer()
        if (verbose): print("Summarizer Starting..")
        summarizedPath = mySummarizer.process(textOutPath)
        if (verbose): print("Summarizer Finished..")

        # textOutPath = r'E:\graduation\output\text\textOutput.json'
        # figOutPath = r'E:\graduation\output\figures\data\doc2ppt.json'
        # summarizedPath = r'E:\graduation\output\text\summarized.json'

        myplacer = imgPlacer()
        if (verbose): print("Image Placer Starting..")
        placerPath = myplacer.process(figOutPath, textOutPath)
        if (verbose): print(f"Image Placer output ={placerPath}..")
        if (verbose): print("Image Placer Finished..")

        # if (verbose): print("Matching Starting..")
        # output = self.matching(summarizedPath, placerPath)
        # if (verbose): print("Matching Finished..")
        print("#OUTPUT_START_HERE")
        print(json.dumps([summarizedPath, placerPath]))
        return [summarizedPath, placerPath]

    # def matching(self, summarizedPath, heading_img):
    #     f = open(summarizedPath)
    #     allSummarizedText = json.load(f)
    #     f.close()

    #     f = open(heading_img)
    #     allHeadingImgs = json.load(f)
    #     f.close()
    #     output = []


if __name__ == '__main__':
    # print(sys.argv)
    if len(sys.argv) == 1:
        raise "No path "
    if not os.path.exists(os.path.dirname(sys.argv[1])):
        raise "invalid path"
    path = sys.argv[-1]
    process.process(path)
