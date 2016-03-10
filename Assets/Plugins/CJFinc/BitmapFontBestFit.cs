using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace CJFinc {

	public class BitmapFontBestFit : MonoBehaviour {
		public int base_font_size;
		public Vector2 base_size;
		public int min_size = 10;
		public int max_size = 100;

		float base_size_area;
		RectTransform rt;
		Text txt;

		void Start () {
			base_size_area = base_size.x * base_size.y;
			InitVars();
		}

		void InitVars() {
			rt = GetComponent<RectTransform>();
			txt = GetComponent<Text>();
		}

		void Update () {
			Vector3 size = rt.rect.size;

			float font_ratio = size.x * size.y / base_size_area;
			if (font_ratio > 1)
				font_ratio = 1+(font_ratio-1)/4;

			int new_font_size = (int)(base_font_size * font_ratio);
			if (new_font_size <= max_size && new_font_size >= min_size)
				txt.fontSize = new_font_size;
		}

		public void SaveBaseSize() {
			InitVars();
			Vector3 size = rt.rect.size;
			base_size = new Vector2(size.x, size.y);
			base_font_size = txt.fontSize;
		}
	}
}
