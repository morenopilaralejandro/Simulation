EXTERNAL GetItemName(item_id)
EXTERNAL HasItem(item_id)
EXTERNAL GetItemCount(item_id)
EXTERNAL GiveItem(item_id, count)
EXTERNAL RemoveItem(item_id, count)
EXTERNAL GetGold()
EXTERNAL GiveGold(amount)
EXTERNAL RemoveGold(amount)
EXTERNAL PlaySFX(sfx_name)
EXTERNAL SetGameFlag(flag_name, value)
EXTERNAL TriggerEvent(event_name)
EXTERNAL GetLocalizedText(key)
EXTERNAL GetCharacterName(char_id)

VAR player_name = "Hero"
VAR item_name = ""
VAR item_count = 0
VAR gold_amount = 100

VAR chest_item_name = ""
VAR chest_item_count = 0

INCLUDE inknpcs/inknpc_test.ink
