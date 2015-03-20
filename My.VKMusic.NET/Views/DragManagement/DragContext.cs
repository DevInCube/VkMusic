using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.VKMusic.Views.DragManagement
{
    public class DragContext
    {

        public ADragVM Item { get; private set; }
        public DragActionType DragAction { get; private set; }
        public IList SourceList { get; private set; }

        public DragContext(ADragVM item, IList source, DragActionType action)
        {
            this.Item = item;
            this.DragAction = action;
            this.SourceList = source;
        }
    }
}
