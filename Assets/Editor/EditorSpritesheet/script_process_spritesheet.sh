#!/bin/bash
set -euo pipefail

SCRIPT1="./script_process_spritesheet_by_animation.sh"
SCRIPT2="./script_process_spritesheet_by_animation_and_item.sh"

INPUT_DIR="/media/gamedisk/Workspace/ProjectsWorkspace/Simulation/LpcDownloaded"
#OUTPUT_DIR="/media/gamedisk/Workspace/ProjectsWorkspace/Simulation/AssetsTest/"
OUTPUT_DIR="/home/alejandro/unity-workspace/Simulation/Assets/Sprites/SpritesCharacter/"

# =========================
# CONFIG 1 - KIT
# =========================
"$SCRIPT1" \
  -i "${INPUT_DIR}/LpcDownloadedKits" \
  -o "${OUTPUT_DIR}SpritesCharacterKit/" \
  -p "sprite-character" \
  -b "SpritesCharacter"

# =========================
# CONFIG 2 - BODY
# =========================
"$SCRIPT1" \
  -i "${INPUT_DIR}/LpcDownloadedBodyRecolor" \
  -o "${OUTPUT_DIR}SpritesCharacterBody/" \
  -p "sprite-character-body" \
  -b "SpritesCharacterBody"

# =========================
# CONFIG 3 - HAIR
# =========================
"$SCRIPT2" \
  -i "${INPUT_DIR}/LpcDownloadedHair" \
  -o "${OUTPUT_DIR}SpritesCharacterHair/" \
  -p "sprite-character" \
  -b "SpritesCharacter"

# =========================
# CONFIG 4 - Wings
# =========================
"$SCRIPT2" \
  -i "${INPUT_DIR}/LpcDownloadedWings" \
  -o "${OUTPUT_DIR}SpritesCharacterWings/" \
  -p "sprite-character" \
  -b "SpritesCharacter"


# =========================
# CONFIG 3 - OPTIONAL EXTRA (example)
# =========================
# "$SCRIPT" \
#   -i "$INPUT_DIR" \
#   -o "/some/other/path/" \
#   -p "sprite-character-armor" \
#   -b "SpritesCharacterArmor"

find "${OUTPUT_DIR}SpritesCharacterHair/" -type d -empty

echo "All sprite batches processed."
