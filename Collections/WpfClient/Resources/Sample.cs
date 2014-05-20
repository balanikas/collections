class Sample
{
    public void SlowCalculation()
    {
        for (int i = 0; i < 10000; i++)
        {
            //do something
        }
    }

    public void FastCalculation()
    {
        return;
    }
}

