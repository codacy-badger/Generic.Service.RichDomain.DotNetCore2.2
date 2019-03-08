using System;
using Generic.Repository.Enum;

namespace Generic.Repository.Extension.Attributes
{
    public class LambdaAttribute : Attribute
    {
        public LambdaAttribute()
        {
            MergeOption = LambdaMerge.And;
            MethodOption = LambdaMethod.Equals;
        }
        public LambdaMerge MergeOption { get; set; }
        public LambdaMethod MethodOption { get; set; }

        public string EntityPropertyName { get; set; }
    }
}