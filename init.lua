-- Lua环境初始化

import('RingEngine', 'RingEngine.Runtime')
import('RingEngine', 'RingEngine.Runtime.Effect')
import('RingEngine', 'RingEngine.Runtime.Script')
import('RingEngine', 'RingEngine.Runtime.Storage')

-- Placements
farleft = Placement(0.0, 200.0, 0.5)
farmiddle = Placement(700.0, 200.0, 0.5)
farright = Placement(1400.0, 200.0, 0.5)
left = Placement(0.0, 200.0, 0.8)
middle = Placement(550.0, 200.0, 0.8)
right = Placement(1100.0, 200.0, 0.8)
nearleft = Placement(0.0, 200.0, 1.0)
nearmiddle = Placement(450.0, 200.0, 1.0)
nearright = Placement(900.0, 200.0, 1.0)

function GetPlacement(character_name, position)
    local y_base_table = runtime.conifg.YBaseTable;
    local y_base = y_base_table[character_name];
    return 1;
end