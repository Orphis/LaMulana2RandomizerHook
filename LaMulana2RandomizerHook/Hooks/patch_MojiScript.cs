using L2Word;
using MonoMod;

#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
namespace LaMulana2RandomizerHook.Hooks
{
    [MonoModPatch("L2Word.MojiScript")]
    public class patch_MojiScript
    {
        [MonoModIgnore]
        private COMMDATA[] commbuff;

        private extern bool orig_getCommData(string data, int start, int num);
        private bool getCommData(string data, int start, int num)
        {
            bool res = orig_getCommData(data, start, num);

            if (replace_item != null)
            {
                commbuff[0].sbuff.Length = 0;
                commbuff[0].sbuff.Append(replace_item);
                replace_item = null;
            }

            return res;
        }

        private extern void orig_com_take(string data, int raw);
        private void com_take(string data, int raw)
        {
            this.getCommData(data, raw, 3);
            if (this.commbuff[0].sbuff.ToString().Equals("Xelputter"))
            {
                replace_item = "La-Mulana";
            } else
            {
                replace_item = null;
            }

            orig_com_take(data, raw);
        }

        private string replace_item;
    }
}
