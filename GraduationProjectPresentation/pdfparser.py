import jarHandler
import ntpath
import os

OUT_PATH = "e:\graduation\PresentMe\output\\"
IN_PATH = "e:\graduation\PresentMe\papers\\"


class Parser():
    def __init__(self, parsertype) -> None:
        if parsertype not in ['text', 'image']:
            raise (f"Error !! {parsertype} is not a valid parser type!!")
        self.__type = parsertype
        self.__handler = jarHandler.jarHandler(parsertype)

    def process(self, file_path, verbose=False):
        result = None
        file_name_ = ntpath.basename(file_path)
        file_name = file_name_[:-3]
        file_extention = file_name_[-3:]
        #print(f"file nam = {file_name} , file extention = {file_extention}")
        if self.__type == 'text':
            args = ['-f', OUT_PATH + 'text\\textOutput.json', file_path]
            result = OUT_PATH + 'text\\textOutput.json'  #+file_name
            if os.path.exists(result):
                os.remove(result)
        else:
            args = [
                '-d', OUT_PATH + 'figures\data\\', '-m',
                OUT_PATH + 'figures\images\\', file_path
            ]
            result = OUT_PATH + 'figures\data' + '\\' + file_name + 'json'
            if os.path.exists(result):
                os.remove(result)

        outputOfHandler = self.__handler.process(*args)
        if verbose:
            print(outputOfHandler)
        return result


if __name__ == '__main__':

    myparser = Parser('text')
    print(myparser.process(IN_PATH + 'doc2ppt.pdf'))
