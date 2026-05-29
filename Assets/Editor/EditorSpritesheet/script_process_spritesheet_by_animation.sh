#!/bin/bash
set -euo pipefail

# Defaults
INPUT_DIR="$HOME/Downloads"

# Kit Path
OUTPUT_DIR="/home/alejandro/unity-workspace/Simulation/Assets/Sprites/SpritesCharacter/SpritesCharacterKit/"
PREFIX="sprite-character"
BASEFOLDER="SpritesCharacter"

# Body Path
# OUTPUT_DIR="/media/gamedisk/Workspace/ProjectsWorkspace/Simulation/Assets/Sprites/SpritesCharacter/SpritesCharacterBody/"
# PREFIX="sprite-character-body"
# BASEFOLDER="SpritesCharacterBody"

#character


#npc
IGNORE_NAMES=(
    "1h_backslash.png"
    "1h_halfslash.png"
    "1h_slash.png"
    "climb.png"
    "combat.png"
    "hurt.png"
    "jump.png"
    "run.png"
    "shoot.png"
    "sit.png"
    "slash.png"
    "spellcast.png"
    "thrust.png"
    "watering.png"
)

usage() {
  echo "Usage: $0 -i input_dir -o output_dir -p prefix -b basefolder"
  exit 1
}

should_ignore() {
    local name="$1"
    for bad in "${IGNORE_NAMES[@]}"; do
      if [[ "$name" == "$bad" ]]; then
        return 0
      fi
    done
    return 1
  }

while getopts "i:o:p:b:" opt; do
  case "$opt" in
    i) INPUT_DIR="$OPTARG" ;;
    o) OUTPUT_DIR="$OPTARG" ;;
    p) PREFIX="$OPTARG" ;;
    b) BASEFOLDER="$OPTARG" ;;
    *) usage ;;
  esac
done

mkdir -p "$OUTPUT_DIR"
shopt -s nullglob

for zipfile in "$INPUT_DIR"/*.zip; do
  [ -e "$zipfile" ] || continue

  zipname="$(basename "$zipfile" .zip)"
  cap_zipname="$(tr '[:lower:]' '[:upper:]' <<< "${zipname:0:1}")${zipname:1}"
  target_folder="$OUTPUT_DIR/${BASEFOLDER}${cap_zipname}"
  mkdir -p "$target_folder"

  tmpdir="$(mktemp -d)"
  trap 'rm -rf "$tmpdir"' RETURN

  unzip -q "$zipfile" "standard/*.png" -d "$tmpdir"

  if [ -d "$tmpdir/standard" ]; then
    find "$tmpdir/standard" -type f -name "*.png" -print0 |
      while IFS= read -r -d '' png; do
        pngname="$(basename "$png")"

        if should_ignore "$pngname"; then
          continue
        fi

        newname="${PREFIX}-${zipname}-${pngname}"
        cp "$png" "$target_folder/$newname"
      done
  fi

  rm -rf "$tmpdir"
  trap - RETURN
done
