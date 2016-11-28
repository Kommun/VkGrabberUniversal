using System;
using System.Collections.Generic;
using System.Text;
using VkGrabberUniversal.Model;

namespace VkGrabberUniversal.ViewModel
{
    public class GroupViewModel
    {
        public Group Group { get; set; }

        public GroupViewModel(Group group)
        {
            Group = group;
        }
    }
}
