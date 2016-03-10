public class JSONEvent : Deprecated.IEvent
{
	public JSONObject jsonData;
	public string callName;

	public JSONEvent(string callName, JSONObject jsonData)
	{
		this.callName = callName;
		this.jsonData = jsonData;
	}

    string Deprecated.IEvent.GetCallName()
	{
		return callName;
	}

    string Deprecated.IEvent.GetName()
	{
		return this.GetType().ToString();
	}

    object Deprecated.IEvent.GetData()
	{
		return jsonData;
	}
}