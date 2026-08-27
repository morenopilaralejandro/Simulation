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
INCLUDE inknpcs/inknpc_test_match_chain.ink
INCLUDE inknpcs/inknpc_test_shop.ink
INCLUDE inknpcs/inknpc_test_choice_multi.ink
INCLUDE inknpcs/inknpc_test_choice_yesno.ink

INCLUDE inkchests/chest_open.ink
INCLUDE inkchests/chest_empty.ink
INCLUDE inkchests/chest_locked.ink

INCLUDE inknpcs/inknpc_shop_elena.ink
INCLUDE inknpcs/inknpc_shop_alejandra.ink
INCLUDE inknpcs/inknpc_shop_quevedo.ink
INCLUDE inknpcs/inknpc_shop_pichi.ink
INCLUDE inknpcs/inknpc_shop_lidia.ink
INCLUDE inknpcs/inknpc_shop_bea.ink
INCLUDE inknpcs/inknpc_shop_pio.ink
INCLUDE inknpcs/inknpc_shop_alma.ink
INCLUDE inknpcs/inknpc_shop_macarena.ink
INCLUDE inknpcs/inknpc_shop_tieman.ink
INCLUDE inknpcs/inknpc_walkway_girl.ink
