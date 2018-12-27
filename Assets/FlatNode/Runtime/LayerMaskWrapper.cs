namespace FlatNode.Runtime
{
    /// <summary>
    /// 技能编辑器中编辑选择的LayerMask
    /// </summary>
    public class LayerMaskWrapper
    {
        public int layer;

        public static implicit operator LayerMaskWrapper(string stringValue)
        {
            LayerMaskWrapper result = new LayerMaskWrapper {layer = int.Parse(stringValue)};
            return result;
        }

        public override string ToString()
        {
            return layer.ToString();
        }
    }
}
