public class JSONEvent : IEvent
{
	public JSONObject jsonData;
	public string callName;

	public JSONEvent(string callName, JSONObject jsonData)
	{
		this.callName = callName;
		this.jsonData = jsonData;
	}

	string IEvent.GetCallName()
	{
		return callName;
	}
	
	string IEvent.GetName()
	{
		return this.GetType().ToString();
	}
	
	object IEvent.GetData()
	{
		return jsonData;
	}
}