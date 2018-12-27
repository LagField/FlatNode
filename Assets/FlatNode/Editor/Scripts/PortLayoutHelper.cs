using UnityEngine;

namespace FlatNode.Editor
{
    public class PortLayoutHelper
    {
        public Rect layoutRect;
        private const float PortHeight = 20f;
        private const float PortPadding = 25f;

        private int calculatedLayoutCount;
        private Rect lastRect;
        private float xOffset;

        public void SetPosition(Vector2 position)
        {
            layoutRect.position = position;
            layoutRect.x = layoutRect.x + xOffset;
            calculatedLayoutCount = 0;
        }

        public void SetOffset(float xOffset,float viewWidth)
        {
            this.xOffset = xOffset;
            layoutRect.width = viewWidth;
        }

        public static float CalculateHeightByPortCount(int count)
        {
            return (PortHeight + PortPadding) * count;
        }

        /// <summary>
        /// 获取一个新的rect
        /// </summary>
        public Rect GetRect()
        {
            Rect result;
            if (calculatedLayoutCount == 0)
            {
                result = new Rect(layoutRect.x, layoutRect.y, layoutRect.width, PortHeight);
            }
            else
            {
                result = new Rect(layoutRect.x, lastRect.y + PortHeight + PortPadding, layoutRect.width,PortHeight);
            }

            lastRect = result;
            calculatedLayoutCount++;
            return result;
        }
    }
}