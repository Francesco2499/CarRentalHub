using System;
using Frontend.ViewModels;

namespace Frontend.Models;

public record ListItemTemplate(Func<ViewModelBase> CreateInstance, string IconKey, string Label);
