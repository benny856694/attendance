using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace huaanClient
{
    class UtilsJson
    {
        public static string PersonJson64 = "{{\"version\": \"0.9\",\"cmd\": \"add person\",\"id\": \"{0}\",\"name\": \"{1}\",\"role\": 1,\"feature_unit_size\": 0,\"feature_num\": 0,\"feature_data\": [],\"image_num\": 1,\"reg_images\": [{2}],\"norm_image_num\":1,\"norm_images\": [{3}],\"term\": \"forever\", \"term_start\":\"useless\",\"long_card_id\": {4},\"customer_text\": \"{5}\",\"term_start\":\"{6}\",\"term\":\"{7}\"}}";
        public static string PersonJson32 = "{{\"version\": \"0.9\",\"cmd\": \"add person\",\"id\": \"{0}\",\"name\": \"{1}\",\"role\": 1,\"feature_unit_size\": 0,\"feature_num\": 0,\"feature_data\": [],\"image_num\": 1,\"reg_images\": [{2}],\"norm_image_num\":1,\"norm_images\": [{3}],\"term\": \"forever\", \"term_start\":\"useless\", \"wg_card_id\": {4},\"customer_text\": \"{5}\",\"term_start\":\"{6}\",\"term\":\"{7}\"}}";
        public static string PersonJson = "{{\"version\": \"0.9\",\"cmd\": \"add person\",\"id\": \"{0}\",\"name\": \"{1}\",\"role\": 1,\"feature_unit_size\": 0,\"feature_num\": 0,\"feature_data\": [],\"image_num\": 1,\"reg_images\": [{2}],\"norm_image_num\":1,\"norm_images\": [{3}],\"wg_card_id\": 0,\"term\": \"forever\", \"term_start\":\"useless\",\"customer_text\": \"{4}\",\"term_start\":\"{5}\",\"term\":\"{6}\"}}";
        public static string PersonJsonforterm = "{{\"version\": \"0.9\",\"cmd\": \"add person\",\"id\": \"{0}\",\"name\": \"{1}\",\"role\": 1,\"feature_unit_size\": 0,\"feature_num\": 0,\"feature_data\": [],\"image_num\": 1,\"reg_images\": [{2}],\"norm_image_num\":1,\"norm_images\": [{3}],\"wg_card_id\": 0,\"term\": \"{4}\", \"term_start\":\"{5}\"}}";
        public static string deleteJson = "{\"version\": \"0.2\",\"cmd\": \"delete person(s)\",\"flag\": -1,\"id\": \"\"}";
        public static string openJson = "{\"version\": \"0.2\",\"cmd\": \"gpio control\",\"port\": 1,\"ctrl_type\": \"on\",\"ctrl_mode\": \"force\",\"person_id\": \"0001\"}";
        public static string ttsPlay = "{\"version\": \"0.2\",\"cmd\": \"tts play\",\"text\":\"开闸成功\" }";
        public static string ttsPlayEn = "{\"version\": \"0.2\",\"cmd\": \"tts play\",\"text\":\"Successful opening\" }";
        public static string ttsPlayJp = "{\"version\": \"0.2\",\"cmd\": \"tts play\",\"text\":\"ブレーキが成功しました\" }";
        public static string SettingParameters = "{\"version\": \"0.2\",\"cmd\": \"update app params\",\"face\": {\"enable_dereplication\": true,\"derep_timeout \": 3 },\"record\": {\"save_enable\": true,\"save_path\": \"EMMC\"},\"name_list\":{\"auto_clean\":true}}";
        public static string SettingParametersFormat = "{{\"version\": \"0.2\",\"cmd\": \"update app params\",\"device_info\": {{\"addr_name\": \"{0}\"}},\"face\": {{\"enable_dereplication\": true,\"derep_timeout \": 3 }},\"record\": {{\"save_enable\": true,\"save_path\": \"EMMC\"}},\"name_list\":{{\"auto_clean\":true}}}}";
        public static string CameraParameter = "{{\"version\":\"0.2\",\"cmd\":\"update app params\",\"face\":{{\"enable_same_face_reg\":{0},\"enable_alive\":{1},\"body_temperature\":{{\"enable\":{2},\"limit\":{3}}}}},\"led_control\":{{\"led_mode\":{4},\"led_brightness\":{5},\"led_sensitivity\":\"{6}\"}}}}";
        public static string CameraParameter_output_not_matched = "{{\"version\":\"0.2\",\"cmd\":\"update app params\",\"face\":{{\"enable_same_face_reg\":{0},\"output_not_matched\":{1},\"enable_alive\":{2},\"body_temperature\":{{\"enable\":{3},\"limit\":{4}}}}},\"led_control\":{{\"led_mode\":{5},\"led_brightness\":{6},\"led_sensitivity\":\"{7}\"}}}}";
        public static string CameraParameterforlcd = "{{\"version\":\"0.2\",\"cmd\":\"update lcd screensaver\",\"screensaver_mode\":\"{0}\"}}";
        public static string camera_volume = "{{\"cmd\": \"camera volume\",\"method\": \"SET\",\"volume\": {0}}}";
        public static string request_persons = "{{\"cmd\": \"request persons\",\"role\": -1,\"page_no\": 1,\"page_size\": 10,\"normal_image_flag\": 1,\"image_flag\": 1,\"query_mode\": 0,\"condition\": {{\"person_id\": \"{0}\"}}}}";
    }
}
