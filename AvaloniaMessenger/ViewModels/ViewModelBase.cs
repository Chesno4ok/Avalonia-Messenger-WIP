using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ValidationContext = ReactiveUI.Validation.Contexts.ValidationContext;

namespace AvaloniaMessenger.ViewModels;

public class ViewModelBase : ReactiveObject, IValidatableViewModel
{
    public IValidationContext ValidationContext => new ValidationContext();
}
