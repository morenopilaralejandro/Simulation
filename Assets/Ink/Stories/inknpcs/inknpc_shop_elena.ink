=== inknpc_shop_elena ===
Hello #loc:greeting_how #speaker:chara-00090-elena #kit:kit-00005-hope:home:field #mood:happy
+ Buy #loc:choice_buy
    #cmd:open_menu:shop:shop-00005-equiment_a
    -> DONE
+ Sell #loc:choice_sell
    #cmd:open_menu:sell
    -> DONE
+ About you #loc:choice_about_you
    Text #loc:npc_elena_you_0 #speaker:chara-00090-elena #kit:kit-00005-hope:home:field #mood:happy
    -> inknpc_shop_elena
+ About this place #loc:choice_about_place
    Text #loc:npc_elena_place_0 #speaker:chara-00090-elena #kit:kit-00005-hope:home:field #mood:happy
    Text #loc:npc_elena_place_1 #speaker:chara-00090-elena #kit:kit-00005-hope:home:field #mood:happy
    -> inknpc_shop_elena
+ Goodbye #loc:choice_goodbye
    Come back soon #loc:farewell_suspense #speaker:chara-00090-elena #kit:kit-00005-hope:home:field #mood:happy
    -> DONE
