
# import necessary packages
import os
import cv2
import mediapipe as mp
import numpy as np
import json

mp_drawing = mp.solutions.drawing_utils
mp_drawing_styles = mp.solutions.drawing_styles
mp_pose = mp.solutions.pose

# Set the image folder (relative to script location)
IMAGE_DIR = "people"  # folder name
IMAGE_PATHS = [os.path.join(IMAGE_DIR, f) for f in os.listdir(IMAGE_DIR) if f.endswith(".png")]
#print(IMAGE_FILES)

BG_COLOR = (192, 192, 192) # gray

with mp_pose.Pose(
    static_image_mode=True,
    model_complexity=2,
    enable_segmentation=True,
    min_detection_confidence=0.5) as pose:
  for idx, file in enumerate(IMAGE_PATHS):
    image = cv2.imread(file)
    #print("HEREEEEEE")
    #print(image)
    image_height, image_width, _ = image.shape
    # Convert the BGR image to RGB before processing.
    results = pose.process(cv2.cvtColor(image, cv2.COLOR_BGR2RGB))

    if not results.pose_landmarks:
      continue
    print(
        f'Nose coordinates: ('
        f'{results.pose_landmarks.landmark[mp_pose.PoseLandmark.NOSE].x * image_width}, '
        f'{results.pose_landmarks.landmark[mp_pose.PoseLandmark.NOSE].y * image_height})'
    )

  #print(results.pose_landmarks)

    skeleton = {
      "nose": [results.pose_landmarks.landmark[mp_pose.PoseLandmark.NOSE].x * image_width,
                results.pose_landmarks.landmark[mp_pose.PoseLandmark.NOSE].y * image_height,
                results.pose_landmarks.landmark[mp_pose.PoseLandmark.NOSE].z * image_width],

      "left_shoulder": [results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_SHOULDER].x * image_width,
                        results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_SHOULDER].y * image_height,
                        results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_SHOULDER].z * image_width],

      "right_shoulder": [results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_SHOULDER].x * image_width,
                          results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_SHOULDER].y * image_height,
                          results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_SHOULDER].z * image_width],

      "left_elbow": [results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_ELBOW].x * image_width,
                      results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_ELBOW].y * image_height,
                      results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_ELBOW].z * image_width],

      "right_elbow": [results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_ELBOW].x * image_width,
                      results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_ELBOW].y * image_height,
                      results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_ELBOW].z * image_width],

      "left_wrist": [results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_WRIST].x * image_width,
                      results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_WRIST].y * image_height,
                      results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_WRIST].z * image_width],

      "right_wrist": [results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_WRIST].x * image_width,
                      results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_WRIST].y * image_height,
                      results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_WRIST].z * image_width],

      "left_index": [results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_INDEX].x * image_width,
                      results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_INDEX].y * image_height,
                      results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_INDEX].z * image_width],

      "right_index": [results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_INDEX].x * image_width,
                      results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_INDEX].y * image_height,
                      results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_INDEX].z * image_width],

      "left_hip": [results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_HIP].x * image_width,
                    results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_HIP].y * image_height,
                    results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_HIP].z * image_width],

      "right_hip": [results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_HIP].x * image_width,
                    results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_HIP].y * image_height,
                    results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_HIP].z * image_width],

      "left_knee": [results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_KNEE].x * image_width,
                    results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_KNEE].y * image_height,
                    results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_KNEE].z * image_width],

      "right_knee": [results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_KNEE].x * image_width,
                      results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_KNEE].y * image_height,
                      results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_KNEE].z * image_width],

      "left_ankle": [results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_ANKLE].x * image_width,
                      results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_ANKLE].y * image_height,
                      results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_ANKLE].z * image_width],

      "right_ankle": [results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_ANKLE].x * image_width,
                      results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_ANKLE].y * image_height,
                      results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_ANKLE].z * image_width],

      "left_foot_index": [results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_FOOT_INDEX].x * image_width,
                          results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_FOOT_INDEX].y * image_height,
                          results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_FOOT_INDEX].z * image_width],

      "right_foot_index": [results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_FOOT_INDEX].x * image_width,
                            results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_FOOT_INDEX].y * image_height,
                            results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_FOOT_INDEX].z * image_width],
    }


    # Save to skeleton#.json
    json_filename = f"skeleton{idx+1}.json"
    with open(json_filename, 'w') as f:
        json.dump(skeleton, f, indent=4)

    print(f"Saved {json_filename}")