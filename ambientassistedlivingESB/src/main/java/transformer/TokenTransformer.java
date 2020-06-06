package transformer;

import java.io.IOException;
import java.util.LinkedHashMap;
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
/**
 * @author Alejandro Díaz Sadoc
 */

@ContainsTransformerMethods
public class TokenTransformer extends AbstractMessageTransformer
{
    @Override
    public synchronized Map<String, Object> transformMessage(MuleMessage message, String outputEncoding)
            throws TransformerException {

        Map<String, Object> eventMap = new LinkedHashMap<String, Object>();
        ObjectMapper mapper = new ObjectMapper();
        JsonNode jsonNode = null;
       
        try {
 
            jsonNode = mapper.readTree(message.getPayloadAsString());

            Map<String, Object> eventPayload = new LinkedHashMap<String, Object>();
          
            eventPayload.put("access_token", jsonNode.get("access_token").asText());
            eventPayload.put("expires_in", jsonNode.get("expires_in").asInt());
            eventPayload.put("type", jsonNode.get("token_type").asText());
            
            eventMap.put("TokenPayload", eventPayload);
           
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
 
        System.out.println("Token created: " + eventMap);
        return eventMap;
    }
}
