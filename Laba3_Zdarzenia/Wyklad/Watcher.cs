var countdown = new Countdown(3);

countdown.Tick += (sender, e) => Console.WriteLine($"{e.Remaining}...");
countdown.Finished += OnFinished;

countdown.Run();
countdown.Finished -= OnFinished; // unsubscribing


// countdown.Finished(countdown, EventArgs.Empty); // error CS0070: only the owner can raise the event

static void OnFinished(object? sender, EventArgs e) => Console.WriteLine("Liftoff!");

public class TickEventArgs(int remaining) : EventArgs
{   
    
    public int Remaining { get; } = remaining;
}

public class Countdown(int start)
{
    public event EventHandler<TickEventArgs>? Tick;
    public event EventHandler? Finished;

    public void Run()
    {
        for (int i = start; i > 0; i--)
        {
            OnTick(new TickEventArgs(i));
        }
        OnFinished(EventArgs.Empty);
    }

    // null when nobody has subscribed - hence ?.Invoke
    protected virtual void OnTick(TickEventArgs e) => Tick?.Invoke(this, e);
    protected virtual void OnFinished(EventArgs e) => Finished?.Invoke(this, e);
}