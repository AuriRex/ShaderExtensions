# ShaderExtensions

**A Mod that adds (mapable!) screen space shaders onto the player camera!**

very crude, user unfriendly and wip how to use / example for mappers:

## info.dat
```
"_difficultyBeatmapSets" : [
{
  "_beatmapCharacteristicName" : "Standard",
  "_difficultyBeatmaps" : [
    {
      "_difficulty" : "Expert",
      "_difficultyRank" : 7,
      "_beatmapFilename" : "ExpertStandard.dat",
      "_noteJumpMovementSpeed" : 16,
      "_noteJumpStartBeatOffset" : 0,
      "_customData" : {
        "_difficultyLabel" : "Shader Test",
        "_requirements" : [
          "Shader Extensions"
        ],
      }
    }
 ],
}
```

## ExpertStandard.dat
```
"_customData": {
    "_customEvents": [{
            "_time": 4,
            "_type": "Shader",
            "_data": {
                "_shaders": [{
                        "_id": "PX1",
                        "_ref": "pixelate",
                        "_clearAfterDone": true,
                        "_props": [{
                                "_prop": "_height",
                                "_duration": 5,
                                "_easing": "easeInOutBounce",
                                "_value": [
                                    [2048,0],
                                    [128,0.5],
                                    [2048,1]
                                ]
                            },
                            {
                                "_prop": "_length",
                                "_duration": 5,
                                "_easing": "easeInOutBounce",
                                "_value": [
                                    [2048,0],
                                    [128,0.5],
                                    [2048,1]
                                ]
                            }
                        ]
                    }
                ]
            }
        },
        {
            "_time": 10,
            "_type": "Shader",
            "_data": {
                "_shaders": [{
                    "_id": "GS1",
                    "_ref": "fms_cat_glitch",
                    "_props": [{
                        "_prop": "_amp",
                        "_duration": 10,
                        "_value": [
                            [0,0.0, "easeInOutCirc"],
                            [0.04,0.05, "easeInOutCirc"],
                            [0,0.1, "easeInOutCirc"],
                            [0.04,0.15, "easeInOutCirc"],
                            [0,0.2, "easeInOutCirc"],
                            [0.04,0.25, "easeInOutCirc"],
                            [0,0.3, "easeInOutCirc"],
                            [0.04,0.35, "easeInOutCirc"],
                            [0,0.4, "easeInOutCirc"],
                            [0.04,0.45, "easeInOutCirc"],
                            [0,0.5, "easeInOutCirc"],
                            [0.04,0.55, "easeInOutCirc"],
                            [0,0.6, "easeInOutCirc"],
                            [0.04,0.65, "easeInOutCirc"],
                            [0,0.7, "easeInOutCirc"],
                            [0.04,0.75, "easeInOutCirc"],
                            [0,0.8, "easeInOutCirc"],
                            [0.04,0.85, "easeInOutCirc"],
                            [0,0.9, "easeInOutCirc"],
                            [0.04,0.95, "easeInOutCirc"],
                            [0,1, "easeInOutCirc"]
                        ]
                    }]
                }]
            }
        },
        {
            "_time": 20,
            "_type": "Shader",
            "_data": {
                "_shaders": [{
                    "_id": "FC1",
                    "_ref": "fancy_color",
                    "_props": [{
                        "_prop": "_strength",
                        "_duration": 5,
                        "_value": [
                            [0.1,0],
                            [1,0.75],
                            [0,1]
                        ]
                    }]
                }]
            }
        },
        {
            "_time": 25,
            "_type": "ShaderClear",
            "_data": {
                "_clearID": "GS1",
                "_ref": "fms_cat_glitch"
            }
        },
        {
            "_time": 25,
            "_type": "Shader",
            "_data": {
                "_shaders": [{
                    "_id": "ROT1",
                    "_ref": "rotate",
                    "_props": [{
                        "_prop": "_RotationValue",
                        "_duration": 30,
                        "_value": [
                            [0,0],
                            [1,0.5],
                            [0,1]
                        ]
                    }]
                }]
            }
        },
        {
            "_time": 55,
            "_type": "ShaderClear",
            "_data": {
                "_clearID": "*"
            }
        }
    ]
}
```
