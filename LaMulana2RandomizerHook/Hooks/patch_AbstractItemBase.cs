using MonoMod;
using System;
using UnityEngine;

#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
namespace LaMulana2RandomizerHook.Hooks
{
    
    [MonoModPatch("global::AbstractItemBase")]
    abstract class patch_AbstractItemBase : AbstractItemBase
    {
        public string originalItemLabel;
    }

    [MonoModPatch("global::L2Flag.L2FlagBox")]
    class patch_L2FlagBox : L2Flag.L2FlagBox
    {
        [MonoModPublic]
        public L2Flag.L2FlagBase flgBaseL;

        [MonoModPublic]
        public L2Flag.L2FlagBase flgBaseR;
    }

    [MonoModPatch("global::TreasureBoxScript")]
    class patch_TreasureBoxScript
    {
        [MonoModIgnore]
        public GameObject itemObj;

        [MonoModIgnore]
        public L2Flag.L2FlagBoxParent[] openFlags;
        
        public extern void orig_groundInit();
        public void groundInit()
        {
            if (itemObj != null)
            {
                string targetItem = null;

                patch_AbstractItemBase itemBase = (patch_AbstractItemBase) itemObj.GetComponent<AbstractItemBase>();
                if (itemBase.originalItemLabel.Length == 0)
                {
                    itemBase.originalItemLabel = itemBase.itemLabel;
                }

                if (itemBase.originalItemLabel.Equals("Shell Horn"))
                {
                    targetItem = "Lamp";
                } else if(itemBase.originalItemLabel.Equals("Holy Grail"))
                {
                    targetItem = "Feather";
                }

                if (targetItem != null)
                {
                    ItemData originalItemData = L2SystemCore.getItemData(itemBase.originalItemLabel);

                    itemBase.setItemParameter(targetItem, 1);
                    ItemData updatedItemData = L2SystemCore.getItemData(targetItem);
                    Sprite targetSprite = L2SystemCore.getMapIconSprite(updatedItemData);

                    SpriteRenderer spriteRenderer = itemObj.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = targetSprite;

                    foreach (L2Flag.L2FlagBoxEnd flag in itemBase.itemGetFlags)
                    {
                        if (flag.calcu == L2Flag.CALCU.EQR && flag.seet_no1 == 2 && flag.flag_no1 == (int) originalItemData.getItemName())
                        {
                            flag.flag_no1 = (int)updatedItemData.getItemName();
                        }
                    }

                    foreach (L2Flag.L2FlagBoxParent flagBoxParent in itemBase.itemActiveFlag)
                    {
                        foreach (L2Flag.L2FlagBox origFlagBox in flagBoxParent.BOX)
                        {
                            patch_L2FlagBox flagBox = (patch_L2FlagBox)origFlagBox;

                            if (flagBox.seet_no1 == 2 && flagBox.flag_no1 == (int)originalItemData.getItemName())
                            {
                                flagBox.flag_no1 = (int)updatedItemData.getItemName();
                            }
                        }
                    }

                    foreach(L2Flag.L2FlagBoxParent flagBoxParent in openFlags)
                    {
                        foreach(L2Flag.L2FlagBox origFlagBox in flagBoxParent.BOX)
                        {
                            patch_L2FlagBox flagBox = (patch_L2FlagBox) origFlagBox;

                            if (flagBox.seet_no1 == 2 && flagBox.flag_no1 == (int)originalItemData.getItemName())
                            {
                                if (flagBox.flgBaseL != null)
                                {
                                    flagBox.flgBaseL.setFlagName(targetItem);
                                }

                                flagBox.flag_no1 = (int)updatedItemData.getItemName();
                            }
                        }
                    }
                }
            }

            orig_groundInit();
        }
    }
}
