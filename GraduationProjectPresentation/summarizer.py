import torch
import json
import pandas as pd
from transformers import T5Tokenizer, T5ForConditionalGeneration, T5Config


class Summarizer:
    def __init__(self, path='t5-base'):
        self.model = T5ForConditionalGeneration.from_pretrained(path).to(
            'cuda:0')
        self.tokenizer = T5Tokenizer.from_pretrained('t5-base')
        self.file = ''
        #self.device = torch.device('cpu')
        self.final_output = []

    def process(self, file):
        f = open(file, encoding="utf8")
        self.file = json.load(f)
        f.close()
        # self.file = pd.read_json(file)
        s = []
        for i in range(len(self.file["metadata"]["sections"])):
            s.append((self.file["metadata"]["sections"][i]["text"]))
        for i in range(len(s)):
            preprocess_text = s[i].strip().replace("\n", "")
            t5_prepared_Text = "summarize: " + preprocess_text
            tokenized_text = self.tokenizer.encode(
                t5_prepared_Text,
                max_length=1000,
                pad_to_max_length=True,
                return_tensors="pt").to('cuda:0')  #.to(self.device)

            # summmarize
            summary_ids = self.model.generate(tokenized_text,
                                              num_beams=4,
                                              no_repeat_ngram_size=2,
                                              min_length=5,
                                              max_length=10000,
                                              early_stopping=True)

            output = self.tokenizer.decode(summary_ids[0],
                                           skip_special_tokens=True)
            self.final_output.append(output)
        for i in range(len(self.file["metadata"]["sections"])):
            self.file["metadata"]["sections"][i]["text"] = self.final_output[i]
        #return self.file["metadata"]
        out_file = r'e:\graduation\output\text\summarized.json'
        with open(out_file, "w") as f:
            json.dump(self.file, f)
        return out_file


if __name__ == '__main__':
    model = Summarizer()
    output = model.process(r'E:\graduation\output\text\doc2ppt.pdf.json')
    # for i in range(len(output["sections"])):
    #     print(output["sections"][i])
    print(output)