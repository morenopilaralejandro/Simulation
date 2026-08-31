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

INCLUDE inknpcs/inknpc_priest.ink

INCLUDE inknpcs/inknpc_street_brown_0.ink
INCLUDE inknpcs/inknpc_street_brown_1.ink
INCLUDE inknpcs/inknpc_park_0.ink
INCLUDE inknpcs/inknpc_park_1.ink
INCLUDE inknpcs/inknpc_walkway_girl.ink
INCLUDE inknpcs/inknpc_park_2.ink
INCLUDE inknpcs/inknpc_park_3.ink
INCLUDE inknpcs/inknpc_street_liner_0.ink
INCLUDE inknpcs/inknpc_street_liner_1.ink
INCLUDE inknpcs/inknpc_island_0.ink
INCLUDE inknpcs/inknpc_island_1.ink

INCLUDE inknpcs/inknpc_boss_dam_0.ink
INCLUDE inknpcs/inknpc_boss_dam_1.ink
INCLUDE inknpcs/inknpc_boss_dam_2.ink
INCLUDE inknpcs/inknpc_boss_dam_3.ink
INCLUDE inknpcs/inknpc_boss_train_0.ink
INCLUDE inknpcs/inknpc_boss_train_1.ink
INCLUDE inknpcs/inknpc_boss_train_2.ink
INCLUDE inknpcs/inknpc_boss_train_3.ink
INCLUDE inknpcs/inknpc_boss_power_plant_0.ink
INCLUDE inknpcs/inknpc_boss_power_plant_1.ink
INCLUDE inknpcs/inknpc_boss_power_plant_2.ink
INCLUDE inknpcs/inknpc_boss_power_plant_3.ink
INCLUDE inknpcs/inknpc_boss_volcano_0.ink
INCLUDE inknpcs/inknpc_boss_volcano_1.ink
INCLUDE inknpcs/inknpc_boss_volcano_2.ink
INCLUDE inknpcs/inknpc_boss_volcano_3.ink
INCLUDE inknpcs/inknpc_boss_final_0.ink

INCLUDE inknpcs/inknpc_boru_0.ink
INCLUDE inknpcs/inknpc_boru_1.ink
INCLUDE inknpcs/inknpc_are_0.ink
INCLUDE inknpcs/inknpc_are_1.ink
INCLUDE inknpcs/inknpc_satu_0.ink
INCLUDE inknpcs/inknpc_satu_1.ink
INCLUDE inknpcs/inknpc_porti_0.ink
INCLUDE inknpcs/inknpc_porti_1.ink
INCLUDE inknpcs/inknpc_pani_0.ink
INCLUDE inknpcs/inknpc_pani_1.ink
INCLUDE inknpcs/inknpc_apa_0.ink
INCLUDE inknpcs/inknpc_apa_1.ink
INCLUDE inknpcs/inknpc_hierro_0.ink
INCLUDE inknpcs/inknpc_hierro_1.ink

INCLUDE inkprop/inkprop_test.ink
INCLUDE inkprop/inkprop_skeleton.ink
INCLUDE inkprop/inkprop_simulation_node_0.ink
INCLUDE inkprop/inkprop_simulation_node_1.ink
INCLUDE inkprop/inkprop_dam_panel.ink
INCLUDE inkprop/inkprop_white_room.ink
