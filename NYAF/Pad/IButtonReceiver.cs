public interface IButtonReceiver
{
    void getButtonPressedInfo(PadActionSet.button buttonPressed, bool isPressed);

    string getOurName();

    void updateOurGfx();

    void padActived();

    void padDeactivated();
}
