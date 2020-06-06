package transformer;
 
import java.io.IOException;
import java.text.DecimalFormat;
import java.text.NumberFormat;
import java.util.LinkedHashMap;
import java.util.Locale;
import java.util.Map;

import org.codehaus.jackson.JsonGenerationException;
import org.codehaus.jackson.JsonNode;
import org.codehaus.jackson.JsonParseException;
import org.codehaus.jackson.map.JsonMappingException;
import org.codehaus.jackson.map.ObjectMapper;
import org.mule.api.MuleMessage;
import org.mule.api.annotations.ContainsTransformerMethods;
import org.mule.api.transformer.TransformerException;
import org.mule.transformer.AbstractMessageTransformer;
 
 
@ContainsTransformerMethods
public class JsonToPruebaEventTransformer extends AbstractMessageTransformer
{
    static DecimalFormat df2 = new DecimalFormat("#,00");
   
    @Override
    public synchronized Map<String, Object> transformMessage(MuleMessage message, String outputEncoding)
            throws TransformerException {
 
        // LinkedHashMap will iterate in the order in which the entries were put into the map
        Map<String, Object> eventMap = new LinkedHashMap<String, Object>();
        ObjectMapper mapper = new ObjectMapper();
        JsonNode jsonNode = null;
       
        try {
 
            jsonNode = mapper.readTree(message.getPayloadAsString());
            
            
            
            // Event payload is a nested map
            Map<String, Object> eventPayload = new LinkedHashMap<String, Object>();
            eventPayload.put("hora", jsonNode.get("Hora").asInt());
            eventPayload.put("classes", jsonNode.get("Classes").getTextValue());
            //eventPayload.put("longitude", jsonNode.get("feeds").path(0).get("field2").asDouble());
            //eventPayload.put("idSensor", jsonNode.get("feeds").path(0).get("field3").asInt()); 
            //eventPayload.put("CO", Double.valueOf(jsonNode.get("feeds").path(0).get("field4").getTextValue().replace(",", ".")));//NumberFormat.getNumberInstance(Locale.FRANCE).parse(jsonNode.get("feeds").path(0).get("field4").getTextValue()));
            //eventPayload.put("Temperature", Double.valueOf(jsonNode.get("feeds").path(0).get("field5").getTextValue().replace(",", ".")));
            //eventPayload.put("Humidity", Double.valueOf(jsonNode.get("feeds").path(0).get("field6").getTextValue().replace(",", ".")));
           
            
            eventMap.put("PersonaEvent", eventPayload);
 
           
        } catch (JsonGenerationException e) {
            e.printStackTrace();
        } catch (JsonMappingException e) {
            e.printStackTrace();
        } catch (JsonParseException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        } catch (Exception e) {
            e.printStackTrace();
        }
 
        //System.out.println("GasEvent created: " + eventMap);
        return eventMap;
    }
}