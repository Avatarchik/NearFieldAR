#if UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_5
#define UNITY_FEATURE_UGUI
#endif

#if UNITY_FEATURE_UGUI
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine;

[AddComponentMenu("AVPro Live Camera/uGUI Component")]
public class AVProLiveCameraUGUIComponent : UnityEngine.UI.MaskableGraphic
{
	[SerializeField]
	public AVProLiveCamera m_liveCamera;

	[SerializeField]
	public Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);
	
	[SerializeField]
	public bool _setNativeSize = false;

	[SerializeField]
	public Texture _defaultTexture;

	private int _lastWidth;
	private int _lastHeight;

	protected AVProLiveCameraUGUIComponent()
	{ }


	/// <summary>
	/// Returns the texture used to draw this Graphic.
	/// </summary>
	public override Texture mainTexture
	{
		get
		{
			Texture result = Texture2D.whiteTexture;
			if (HasValidTexture())
			{
				result = m_liveCamera.OutputTexture;
			}
			else
			{
				if (_defaultTexture != null)
				{
					result = _defaultTexture;
				}
			}
			return result;
		}
	}

	public bool HasValidTexture()
	{
		return (m_liveCamera != null && m_liveCamera.OutputTexture != null);
	}
	void Update()
	{
		if (mainTexture == null)
			return;

		if (_setNativeSize)
			SetNativeSize();
		if (HasValidTexture())
		{
			if (mainTexture.width != _lastWidth ||
			    mainTexture.height != _lastHeight)
			{
				_lastWidth = mainTexture.width;
				_lastHeight = mainTexture.height;
				SetVerticesDirty();
			}
		}
		SetMaterialDirty();
	}

	/// <summary>
	/// Texture to be used.
	/// </summary>
	public AVProLiveCamera source
	{
		get
		{
			return m_liveCamera;
		}
		set
		{
			if (m_liveCamera == value)
				return;

			m_liveCamera = value;
			//SetVerticesDirty();
			SetMaterialDirty();
		}
	}

	/// <summary>
	/// UV rectangle used by the texture.
	/// </summary>
	public Rect uvRect
	{
		get
		{
			return m_UVRect;
		}
		set
		{
			if (m_UVRect == value)
				return;
			m_UVRect = value;
			SetVerticesDirty();
		}
	}

	/// <summary>
	/// Adjust the scale of the Graphic to make it pixel-perfect.
	/// </summary>

	[ContextMenu("Set Native Size")]
	public override void SetNativeSize()
	{
		Texture tex = mainTexture;
		if (tex != null)
		{
			int w = Mathf.RoundToInt(tex.width * uvRect.width);
			int h = Mathf.RoundToInt(tex.height * uvRect.height);
			rectTransform.anchorMax = rectTransform.anchorMin;
			rectTransform.sizeDelta = new Vector2(w, h);
		}
	}

	/// <summary>
	/// Update all renderer data.
	/// </summary>
	protected override void OnFillVBO(List<UIVertex> vbo)
	{
		Texture tex = mainTexture;

		int texWidth = 4;
		int texHeight = 4;

		if (HasValidTexture())
		{

		}
		if (tex != null)
		{
			texWidth = tex.width;
			texHeight = tex.height;
		}		{
			Vector4 v = Vector4.zero;

			int w = Mathf.RoundToInt(tex.width * uvRect.width);
			int h = Mathf.RoundToInt(tex.height * uvRect.height);

			float paddedW = ((w & 1) == 0) ? w : w + 1;
			float paddedH = ((h & 1) == 0) ? h : h + 1;

			v.x = 0f;
			v.y = 0f;
			v.z = w / paddedW;
			v.w = h / paddedH;

			v.x -= rectTransform.pivot.x;
			v.y -= rectTransform.pivot.y;
			v.z -= rectTransform.pivot.x;
			v.w -= rectTransform.pivot.y;

			v.x *= rectTransform.rect.width;
			v.y *= rectTransform.rect.height;
			v.z *= rectTransform.rect.width;
			v.w *= rectTransform.rect.height;

			vbo.Clear();

			var vert = UIVertex.simpleVert;

			vert.position = new Vector2(v.x, v.y);
			vert.uv0 = new Vector2(m_UVRect.xMin, m_UVRect.yMin);
			vert.color = color;
			vbo.Add(vert);

			vert.position = new Vector2(v.x, v.w);
			vert.uv0 = new Vector2(m_UVRect.xMin, m_UVRect.yMax);
			vert.color = color;
			vbo.Add(vert);

			vert.position = new Vector2(v.z, v.w);
			vert.uv0 = new Vector2(m_UVRect.xMax, m_UVRect.yMax);
			vert.color = color;
			vbo.Add(vert);

			vert.position = new Vector2(v.z, v.y);
			vert.uv0 = new Vector2(m_UVRect.xMax, m_UVRect.yMin);
			vert.color = color;
			vbo.Add(vert);
		}
	}
}

#endif