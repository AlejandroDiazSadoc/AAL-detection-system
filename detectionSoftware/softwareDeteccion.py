import numpy as np
import os
import six.moves.urllib as urllib
import sys
import tarfile
import tensorflow as tf
import zipfile
from collections import defaultdict
from io import StringIO
from matplotlib import pyplot as plt
from PIL import Image
import cv2 as cv2
from json import dumps
import logging
import sys
from dotenv import load_dotenv
load_dotenv('.env')


SERVER = os.getenv("SERVER")
PROTOCOL=os.getenv("PROTOCOL")
MECHANISM=os.getenv("MECHANISM")
USER=os.getenv("USER")
KEYUSER=os.getenv("KEYUSER")
KEY=os.getenv("KEY")
CERT=os.getenv("CERT")
KEYSTORE=os.getenv("KEYSTORE")
VER=os.getenv("VER")



from confluent_kafka import Producer

def delivery_callback(err, msg):
    if err:
        sys.stderr.write('%% El mensaje no se ha envíado: %s\n' % err)
    else:
        sys.stderr.write('%% Mensaje envíado a %s [%d] @ %d\n' %
                             (msg.topic(), msg.partition(), msg.offset()))

producer = Producer({'bootstrap.servers': SERVER,
'security.protocol': PROTOCOL,
'sasl.mechanisms':MECHANISM,
'sasl.username':USER,
'sasl.password':KEYUSER,
'ssl.certificate.location':CERT,
'ssl.keystore.location': KEYSTORE,
'ssl.keystore.password':KEY,
'enable.ssl.certificate.verification':VER})

ficheroSalida = './videoSalida.avi'


if os.path.isfile(ficheroSalida):
    os.remove(ficheroSalida)


cap = cv2.VideoCapture('./movie071.mp4')


frame_width = int(cap.get(3))
frame_height = int(cap.get(4))


out = cv2.VideoWriter(ficheroSalida, cv2.VideoWriter_fourcc('M', 'J', 'P', 'G'),
                      30, (frame_width, frame_height))

sys.path.append("..")


from utils import label_map_util
#from utils import visualization_utils as vis_util
from myOwnVisualizer import myOwnVis

pathModelo = './GRAFOSINFERENCIAS/10Mayo/2'
pathInferenceGraph = pathModelo + '/frozen_inference_graph.pb'
pathLabel = os.path.join('./', 'label_map.pbtxt')
NUM_CLASSES = 25

detection_graph = tf.Graph()
with detection_graph.as_default():
    od_graph_def = tf.GraphDef()
    with tf.gfile.GFile(pathInferenceGraph, 'rb') as fid:
        serialized_graph = fid.read()
        od_graph_def.ParseFromString(serialized_graph)
        tf.import_graph_def(od_graph_def, name='')


label_map = label_map_util.load_labelmap(pathLabel)
categorias = label_map_util.convert_label_map_to_categories(label_map, max_num_classes=NUM_CLASSES, use_display_name=True)
category_index = label_map_util.create_category_index(categorias)

with detection_graph.as_default():
    with tf.Session(graph=detection_graph) as sess:
        # Entradas y salidas para el grafo
        image_tensor = detection_graph.get_tensor_by_name('image_tensor:0')

        # Cajas detectadas en la imagen
        cajas_detectadas = detection_graph.get_tensor_by_name('detection_boxes:0')

        # Nivel de precisión de la detección.
        precision_detecciones = detection_graph.get_tensor_by_name('detection_scores:0')
        clases_detectadas = detection_graph.get_tensor_by_name('detection_classes:0')
        num_detecciones = detection_graph.get_tensor_by_name('num_detections:0')

        while(cap.isOpened()):

            ret, frame = cap.read()
            if ret == True:
                
                imagen_expandida = np.expand_dims(frame, axis=0)

            # Detección
                (cajas, precisiones, clases, num) = sess.run(
                [cajas_detectadas, precision_detecciones, clases_detectadas, num_detecciones],
                feed_dict={image_tensor: imagen_expandida})
  
                #Visualización de los resultados
                detectado=myOwnVis(
                frame,
                np.squeeze(cajas),
                np.squeeze(clases).astype(np.int32),
                np.squeeze(precisiones),
                category_index,
                use_normalized_coordinates=True,
                line_thickness=8)
                
                #Envia mensajes a Kafka
                producer.produce('test', value=dumps(detectado).encode('utf-8'),callback=delivery_callback)
                print(detectado)
               
                out.write(frame)

                # Muestra la imagen con las detecciones
                cv2.imshow('AAL detección', frame)

                # Cierra la ventana cuando se presiona la tecla "q"
                if cv2.waitKey(1) & 0xFF == ord('q'):
                    break
            else:
                break

    cap.release()
    out.release()

    cv2.destroyAllWindows()
