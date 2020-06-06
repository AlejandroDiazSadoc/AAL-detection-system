package voice;

import java.io.IOException;
import java.net.URI;
/**
 * @author Alejandro Díaz Sadoc
 */

public class VoiceSystem {
	
	public void openUrl() throws IOException {
		URI url = URI.create("http://localhost:5000/");
		System.out.println(url.toString());
		java.awt.Desktop.getDesktop().browse(url);
	}

}
