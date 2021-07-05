import json
from pprint import pprint


class imgPlacer:
    def __init__(self):
        pass

    def readImgDate(self, figOutPath):
        with open(figOutPath) as f:
            imgData = json.loads(f.read())

        return imgData

    def readTextData(self, textOutPath):
        with open(textOutPath, encoding='UTF-8') as f:
            textData = json.loads(f.read())

        return textData

    def process(self, figOutPath, textOutPath):
        textData = self.readTextData(textOutPath)
        output = self.matching(figOutPath, textOutPath)
        outputName = textData["name"] + "image_placer.json"

        with open(outputName, 'w') as json_out:
            json.dump(output, json_out)
        return outputName

    def matching(self, figOutPath, textOutPath):
        imgData = self.readImgDate(figOutPath)
        textData = self.readTextData(textOutPath)
        output = []

        for i in imgData:
            caption = (i["caption"])
            dataType = caption.split(" ", 3)[:2]
            dataType = " ".join(dataType)
            if not dataType[-1].isdigit(): dataType = dataType[:-1]
            # print(dataType)
            sections = textData["metadata"]["sections"]
            for section in sections:
                text = section["text"]
                dict = {}
                if dataType in text:
                    dict["heading"] = section["heading"]
                    dict["path"] = i["renderURL"]
                    output.append(dict)

        return output


if __name__ == '__main__':
    a = r"D:\CCE Department\4th CCE\Graduation Project\graduation_project\pdffigures\output\data\ourpapers_NIPS-2017-attention-is-all-you-need-Paper.json"
    b = r"D:\CCE Department\4th CCE\Graduation Project\graduation_project\science-parser\NIPS-2017-attention-is-all-you-need-Paper.pdf.json"

    textOutPath = r'E:\graduation\PresentMe\output\text\textOutput.json'
    figOutPath = r'E:\graduation\PresentMe\output\figures\data\doc2ppt.json'

    x = imgPlacer()
    print(x.process(figOutPath, textOutPath))
