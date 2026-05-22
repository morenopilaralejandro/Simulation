#!/bin/bash
set -euo pipefail

# ============================================================
# Defaults
# ============================================================

INPUT_DIR="$HOME/Downloads"

OUTPUT_DIR="/home/alejandro/unity-workspace/Simulation/Assets/Sprites/SpritesCharacter/SpritesCharacterHair"

PREFIX="sprite-character"
BASEFOLDER="SpritesCharacter"

IGNORE_FOLDERS=(
    "1h_slash"
    "climb"
    "shoot"
    "sit"
    "thrust"
    "watering"
)

# ============================================================
# Usage
# ============================================================

usage() {
  echo "Usage: $0 -i input_dir -o output_dir -p prefix -b basefolder"
  exit 1
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

# ============================================================
# Helpers
# ============================================================

should_ignore_folder() {
    local folder="$1"

    for bad in "${IGNORE_FOLDERS[@]}"; do
        if [[ "$folder" == "$bad" ]]; then
            return 0
        fi
    done

    return 1
}

extract_number() {
    local filename="$1"

    if [[ "$filename" =~ ^([0-9]{3}) ]]; then
        echo "${BASH_REMATCH[1]}"
    else
        echo "999999"
    fi
}

capitalize_first() {
    local s="$1"
    echo "$(tr '[:lower:]' '[:upper:]' <<< "${s:0:1}")${s:1}"
}

# ============================================================
# Main
# ============================================================

mkdir -p "$OUTPUT_DIR"

shopt -s nullglob

for zipfile in "$INPUT_DIR"/*.zip; do
    [ -e "$zipfile" ] || continue

    zipname="$(basename "$zipfile" .zip)"

    IFS='-' read -ra parts <<< "$zipname"

    cap_zipname=""

    for part in "${parts[@]}"; do
        cap_part="$(capitalize_first "$part")"
        cap_zipname+="$cap_part"
    done

    FRONT_FOLDER="$OUTPUT_DIR/${BASEFOLDER}${cap_zipname}Front"
    BACK_FOLDER="$OUTPUT_DIR/${BASEFOLDER}${cap_zipname}Back"

    mkdir -p "$FRONT_FOLDER"
    mkdir -p "$BACK_FOLDER"

    tmpdir="$(mktemp -d)"
    trap 'rm -rf "$tmpdir"' RETURN

    unzip -q "$zipfile" -d "$tmpdir"

    STANDARD_DIR="$tmpdir/$zipname/standard"

    if [ ! -d "$STANDARD_DIR" ]; then
        STANDARD_DIR="$tmpdir/standard"
    fi

    if [ -d "$STANDARD_DIR" ]; then

        for animdir in "$STANDARD_DIR"/*; do

            [ -d "$animdir" ] || continue

            animname="$(basename "$animdir")"

            if should_ignore_folder "$animname"; then
                echo "[IGNORED] $animname"
                continue
            fi

            pngs=( "$animdir"/*.png )

            if [ "${#pngs[@]}" -lt 2 ]; then
                echo "[WARNING] Only 1 png in $animname -> treating as FRONT"

                front_src="${pngs[0]}"

                front_name="${PREFIX}-${zipname}-front-${animname}.png"

                cp "$front_src" "$FRONT_FOLDER/$front_name"

                continue
            fi

            file1="$(basename "${pngs[0]}")"
            file2="$(basename "${pngs[1]}")"

            num1="$(extract_number "$file1")"
            num2="$(extract_number "$file2")"

            if (( 10#$num1 < 10#$num2 )); then
                back_src="${pngs[0]}"
                front_src="${pngs[1]}"
            else
                back_src="${pngs[1]}"
                front_src="${pngs[0]}"
            fi

            front_name="${PREFIX}-${zipname}-front-${animname}.png"
            back_name="${PREFIX}-${zipname}-back-${animname}.png"

            cp "$front_src" "$FRONT_FOLDER/$front_name"
            cp "$back_src" "$BACK_FOLDER/$back_name"

            echo "[COPIED] $animname"
        done
    fi

    rm -rf "$tmpdir"
    trap - RETURN
done

echo "Done."
