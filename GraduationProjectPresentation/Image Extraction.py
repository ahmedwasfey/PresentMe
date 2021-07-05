import json
import cv2
import numpy as np
import os
from pathlib import Path


def process(fpath, file):
    image = cv2.imread(fpath, cv2.IMREAD_UNCHANGED)
    im2 = image.copy()

    if len(im2.shape) == 3:
        img_gray = cv2.cvtColor(im2, cv2.COLOR_BGR2GRAY)
    else:
        img_gray = image.copy()

    _, thresh = cv2.threshold(img_gray, 190, 255,
                              cv2.THRESH_BINARY_INV | cv2.THRESH_OTSU)

    kernel = np.ones((10, 10), np.uint8)

    gradient = cv2.morphologyEx(thresh, cv2.MORPH_GRADIENT, kernel)

    contours, _ = cv2.findContours(image=gradient,
                                   mode=cv2.RETR_EXTERNAL,
                                   method=cv2.CHAIN_APPROX_SIMPLE)
    cv2.drawContours(im2, contours, -1, (0, 255, 0), 1, cv2.LINE_AA)

    cnt = 1
    for contour in contours:
        (x, y, w, h) = cv2.boundingRect(contour)
        # a = cv2.contourArea(contour)
        if h > 200:
            cv2.rectangle(im2, (x, y), (x + w, y + h), (0, 0, 255))
            cropped = image[y:y + h, x:x + w]
            impath = f"img({cnt})_slide({file[:len(file)-4]}).jpg"
            cv2.imwrite(impath, cropped)
            cnt += 1
            impath = os.path.join(subdir, impath)
            data[impath] = {
                "startx": f"{x}",
                "starty": f"{y}",
                "width": f"{w}",
                "hight": f"{h}"
            }

    with open("images.json", "w") as f:
        json.dump(data, f)


inpath = r"E:\graduation\data\data_raw"
outpath = r"E:\graduation\data\extracted_data"

print("\n>>>> [INFO] Extracting image...\n")
for dir, subdirs, files in os.walk(inpath):

    # Create output files
    subdir = outpath + dir[len(inpath):]
    Path(subdir).mkdir(parents=True, exist_ok=True)
    os.chdir(subdir)
    data = {}
    print(dir)
    print("-------------------------------------------")
    for f in files:
        fpath = os.path.join(dir, f)
        if fpath.endswith(".jpg") or fpath.endswith(".jpeg"):
            process(fpath, f)

print("\n>>>> [INFO] Extraction process is done.\n")