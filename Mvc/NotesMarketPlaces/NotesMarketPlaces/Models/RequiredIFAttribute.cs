using System;

namespace NotesMarketPlaces.Models
{
    internal class RequiredIFAttribute : Attribute
    {
        private string v1;
        private bool v2;
        private string errorMessage;

        public RequiredIFAttribute(string v1, bool v2, string ErrorMessage)
        {
            this.v1 = v1;
            this.v2 = v2;
            errorMessage = ErrorMessage;
        }
    }
}