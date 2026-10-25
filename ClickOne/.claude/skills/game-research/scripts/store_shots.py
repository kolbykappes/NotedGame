#!/usr/bin/env python
"""store_shots.py - download a game's Steam store screenshots to a scratch folder, shrunk for viewing.

Usage: python store_shots.py <appid> <outdir> [--max 8] [--width 960]

Reads the screenshot URLs from the store API, downloads them, shrinks each to --width pixels wide (JPEG) and prints
the file paths. View them with the Read tool (images are shown to you), describe each one, and mark interpretation.
Do NOT copy them into the repo unless the owner asks. Needs Pillow (`pip install pillow`).
"""
import json
import os
import sys
import urllib.request

from PIL import Image


def main():
    args = sys.argv[1:]
    if len(args) < 2:
        print(__doc__)
        sys.exit(1)
    appid, outdir = args[0], args[1]
    n = int(args[args.index("--max") + 1]) if "--max" in args else 8
    width = int(args[args.index("--width") + 1]) if "--width" in args else 960
    os.makedirs(outdir, exist_ok=True)

    req = urllib.request.Request(
        f"https://store.steampowered.com/api/appdetails?appids={appid}&l=english",
        headers={"User-Agent": "competitive-research/1.0"},
    )
    data = json.load(urllib.request.urlopen(req, timeout=30))
    shots = (data.get(appid, {}).get("data", {}) or {}).get("screenshots", [])[:n]
    if not shots:
        print("no screenshots found")
        return
    for i, s in enumerate(shots, 1):
        raw = os.path.join(outdir, f"s{i}_full.jpg")
        urllib.request.urlretrieve(s["path_full"], raw)
        im = Image.open(raw).convert("RGB")
        im.thumbnail((width, width * 9 // 16 + 1))
        small = os.path.join(outdir, f"s{i}.jpg")
        im.save(small, quality=80)
        os.remove(raw)
        print(small, im.size)


if __name__ == "__main__":
    main()
