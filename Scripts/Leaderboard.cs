public class Leaderboard
{
    public int firstPlace;
    public int secondPlace;
    public int thirdPlace;

    public Leaderboard(int first = 0, int second = 0, int third = 0)
    {
        firstPlace = first;

        secondPlace = second;
        if (secondPlace > firstPlace)
        {
            firstPlace = second;
            secondPlace = first;
        }

        thirdPlace = third;
        if (thirdPlace > secondPlace)
        {
            if (thirdPlace > firstPlace)
            {
                int temp1 = firstPlace;
                int temp2 = secondPlace;

                firstPlace = third;
                secondPlace = temp1;
                thirdPlace = temp2;
            }
            else
            {
                secondPlace = third;
                thirdPlace = second;
            }
        }
    }

    public int GetPlaceOnLeaderboard(int score)
    {
        if (score >= firstPlace)
            return 1;

        if (score >= secondPlace)
            return 2;

        if (score >= thirdPlace)
            return 3;

        return 4;
    }

    public void AddToLeaderboard(int score)
    {
        if (score >= firstPlace)
        {
            thirdPlace = secondPlace;
            secondPlace = firstPlace;
            firstPlace = score;
            return;
        }

        if (score >= secondPlace)
        {
            thirdPlace = secondPlace;
            secondPlace = score;
            return;
        }

        if (score >= thirdPlace)
        {
            thirdPlace = score;
            return;
        }
    }
}
