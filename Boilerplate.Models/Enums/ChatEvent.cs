namespace Boilerplate.Models.Enums
{
    public enum ChatEvent
    {
        UserConnected, //when user connects to server
        UserDisconnected, //when user disconnects to server
        UserJoined, //when user joins to group
        UserLeft, //when user leaves from group
        MessageReceived, //when user received message
        MessageEdited, //when user edit message
        MessageDeleted, //when user delete message
    }
}
