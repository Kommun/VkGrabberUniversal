using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using VkGrabberUniversal.Model.Rest;

namespace VkGrabberUniversal.Model
{
    public class AttachmentInfo : DependencyObject
    {
        #region PostProperty        

        /// <summary>
        /// Пост
        /// </summary>
        public Post Post
        {
            get { return (Post)GetValue(PostProperty); }
            set { SetValue(PostProperty, value); }
        }

        public static readonly DependencyProperty PostProperty = DependencyProperty.Register("Post", typeof(Post), typeof(AttachmentInfo), null);


        #endregion

        #region AttachmentProperty        

        /// <summary>
        /// Прикрепленное изображение
        /// </summary>
        public Attachment Attachment
        {
            get { return (Attachment)GetValue(AttachmentProperty); }
            set { SetValue(AttachmentProperty, value); }
        }

        public static readonly DependencyProperty AttachmentProperty = DependencyProperty.Register("Attachment", typeof(Attachment), typeof(AttachmentInfo), null);


        #endregion
    }
}
