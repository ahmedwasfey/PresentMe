from subprocess import *

TEXT_JAR_PATH = 'E:\graduation\PresentMe\packages\science-parser\cli\\target\scala-2.11\science-parse-cli-assembly-3.0.1.jar '
IMAGE_JAR_PATH = 'E:\graduation\PresentMe\packages\pdffigures\\target\scala-2.11\pdffigures2-assembly-0.1.0.jar '


class jarHandler():
    def __init__(self, jartype) -> None:
        if jartype not in ['text', 'image']:
            raise (f"Error !! {jartype} is not a valid JAR type!!")
        self.type = jartype
        self.jar = None

    def process(self, *args):
        if self.type == 'text':
            process = Popen(['java', '-Xmx6g', '-jar', TEXT_JAR_PATH] +
                            list(args),
                            stdout=PIPE,
                            stderr=PIPE)
        elif self.type == 'image':
            # self.jar =
            process = Popen(['java', '-jar', IMAGE_JAR_PATH] + list(args),
                            stdout=PIPE,
                            stderr=PIPE)
        else:
            raise ("Unknown Jar type")
        #print(f"Accessed {self.type} jar successfully ")
        ret = []
        i = 0
        while process.poll() is None:
            i += 1
            if i % 1000 == 0:
                #print(f'still running #{i//1000}....')
                pass
            line = process.stdout.readline()
            # line = line.decode('utf-8')
            if line != b'':
                # print(f"line : {line}")
                ret.append(line[:-1])
                ret.append('\n')

        stdout, stderr = process.communicate()
        # print(stdout , stderr)
        if stdout != b'':
            ret += stdout.decode('utf-8').split('\n')
        if stderr != b'':
            ret += stderr.decode('utf-8').split('\n')
        try:
            ret.remove('')
        except:
            pass
        return ret


if __name__ == "__main__":
    args = ['-o', 'e:\graduation\output\\text\\', 'e:\graduation\papers\\'
            ]  # Any number of args to be passed to the jar file
    # args =['-d', 'e:\graduation\output\\figures\data\p_' ,'-m', 'e:\graduation\output\\figures\images\p_' ,'e:\graduation\papers\\']
    jarer = jarHandler('text')
    result = jarer.process(*args)

    # print (result)
