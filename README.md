# progressmonitor

A library to allow reporting progress of long tasks to the user.


## .Net

Usage example:

```C#
var monitor = new ConsoleProgressMonitor();
using (monitor.ConfigureSteps(1, 4, 5)) {
  monitor.StartStep("First step with weight 1");
  ...
  monitor.StartStep("Second step with weight 4");
  ...
  monitor.Report("Still working...");
  ...
  monitor.StartStep("Third step with weight 5");
  ...
}
```

### Installing

    PM> Install-Package ProgressMonitor 
