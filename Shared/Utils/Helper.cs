using System.ComponentModel.DataAnnotations;

namespace Shared.Utils
{
    /// <summary>
    ///     Helper class to implement validations and other
    /// </summary>
    public static class Helper
    {
        /// <summary>
        ///     Validate whether the model is valid based on it's annotations.
        ///     Return empty string if model is valid, otherwise return errors
        /// </summary>
        /// <param name="model">
        ///     The model to validate
        /// </param>
        public static string ValidateModel(object model)
        {
            var validationContext = new ValidationContext(model, null, null);
            List<ValidationResult> errors = [];

            Validator.TryValidateObject(model, validationContext, errors, true);

            if (errors.Count == 0)
            {
                return "";
            }

            return string.Join(", ", errors.Select(vr => vr.ErrorMessage));
        }
    }
}
