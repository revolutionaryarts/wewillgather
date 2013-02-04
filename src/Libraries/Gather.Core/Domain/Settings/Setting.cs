using System;

namespace Gather.Core.Domain.Settings
{
    public class Setting : BaseEntity
    {
        /// <summary>
        /// Last modified by using user id
        /// </summary>
        public virtual int? LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the last modified date
        /// </summary>
        public virtual DateTime? LastModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the setting
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the setting
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Returns the setting value as a specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T As<T>()
        {
            return CommonHelper.To<T>(Value);
        }
    }
}