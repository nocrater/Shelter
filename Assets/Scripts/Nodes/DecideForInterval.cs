using NPBehave;

public class DecideForInterval : Task
{
    private System.Func<float> function = null;
    private string blackboardKey = null;
    private float seconds = -1f;
    private float randomVariance;

    private readonly System.Action action;

    public float RandomVariance
    {
        get
        {
            return randomVariance;
        }
        set
        {
            randomVariance = value;
        }
    }

    public DecideForInterval(System.Action action, float seconds, float randomVariance) : base("DecideForInterval")
    {
        UnityEngine.Assertions.Assert.IsTrue(seconds >= 0);
        this.action = action;
        this.seconds = seconds;
        this.randomVariance = randomVariance;
    }

    public DecideForInterval(System.Action action, float seconds) : base("DecideForInterval")
    {
        this.action = action;
        this.seconds = seconds;
        this.randomVariance = this.seconds * 0.05f;
    }

    public DecideForInterval(System.Action action, string blackboardKey, float randomVariance = 0f) : base("DecideForInterval")
    {
        this.action = action;
        this.blackboardKey = blackboardKey;
        this.randomVariance = randomVariance;
    }

    public DecideForInterval(System.Action action, System.Func<float> function, float randomVariance = 0f) : base("DecideForInterval")
    {
        this.action = action;
        this.function = function;
        this.randomVariance = randomVariance;
    }

    protected override void DoStart()
    {
        action();
        float seconds = this.seconds;
        if (seconds < 0)
        {
            if (this.blackboardKey != null)
            {
                seconds = Blackboard.Get<float>(this.blackboardKey);
            }
            else if (this.function != null)
            {
                seconds = this.function();
            }
        }
//            UnityEngine.Assertions.Assert.IsTrue(seconds >= 0);
        if (seconds < 0)
        {
            seconds = 0;
        }

        if (randomVariance >= 0f)
        {
            Clock.AddTimer(seconds, randomVariance, 0, onTimer);
        }
        else
        {
            Clock.AddTimer(seconds, 0, onTimer);
        }
    }

    protected override void DoStop()
    {
        Clock.RemoveTimer(onTimer);
        this.Stopped(false);
    }

    private void onTimer()
    {
        Clock.RemoveTimer(onTimer);
        this.Stopped(true);
    }
}