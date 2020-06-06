package esper;

import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.Map;
import java.util.StringJoiner;

import org.mule.api.MuleEventContext;
import org.mule.api.annotations.param.Payload;
import org.mule.api.lifecycle.Callable;

import com.espertech.esper.compiler.client.EPCompileException;
import com.espertech.esper.runtime.client.EPDeployException;
/**
 * @author Alejandro Díaz Sadoc
 */

public class MyEsperClass implements Callable {
	
	@Override
	public Object onCall(final MuleEventContext eventContext) throws Exception {
		new EsperUtils();
		String schema=eventContext.getMessage().getPayload().toString();
		if (!EsperUtils.eventTypeExists("GasEvent")) {
			try {
				EsperUtils.addNewSchema(schema);
				System.out.println("Schema added");
			} catch (EPCompileException | EPDeployException e) {
				e.printStackTrace();
				System.out.println("*** ERROR adding new event type to Esper engine " + e.toString());
			}

			
		}
		return schema;
	}
	
	
}
