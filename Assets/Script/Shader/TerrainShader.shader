Shader "Custom/TerrainShader"
{
	Properties{
		_GrassTex("GrassTexture", 2D) = "White"{}
		_GrassScale("GrassScale", Float) = 1
		_WallTex("WallTexture", 2D) = "White"{}
		_WallScale("WallScale", Float) = 1
		_RockTex("RockTexture", 2D) = "White"{}
		_RockScale("RockScale", Float) = 1
		_TransitionOffset("TransitionOffset", Float) = 1
		_ChunkSize("ChunkSize", Int) = 1
	}

	SubShader{
		Tags{"RenderType" = "Opaque"}
		LOD 200
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _GrassTex;
		sampler2D _RockTex;
		sampler2D _WallTex;
		float _TexScale;
		float _GrassScale;
		float _WallScale;
		float _RockScale;
		



		float _GrassStartHeight;
		float _RockStartHeight;
		float _MountainStartHeight;

		int _ChunkSize;
		float _TransitionOffset;
		int _ChunkPosX;
		int _ChunkPosY;

		int _RightTransitionNeeded;
		sampler2D _RightGrassTex;
		sampler2D _RightWallTex;

		int _FrontTransitionNeeded;
		sampler2D _FrontGrassTex;
		sampler2D _FrontWallTex;

		int _RightFrontTransitionNeeded;
		sampler2D _RightFrontGrassTex;
		sampler2D _RightFrontWallTex;

		

		struct Input {
			float3 worldPos;
			float3 worldNormal;
		};

		void surf(Input IN, inout SurfaceOutputStandard o) {



			float height = IN.worldPos.y;
			float3 pWeight = abs(IN.worldNormal);
			pWeight /= pWeight.x + pWeight.y + pWeight.z;

			float2 positionInChunk = float2(IN.worldPos.x - _ChunkPosX, IN.worldPos.z - _ChunkPosY);
			
			float3 groundColor;

			if (height > _GrassStartHeight) {


				float3 xP = tex2D(_WallTex, (IN.worldPos / _WallScale).yz) * pWeight.x;
				float3 yP = tex2D(_GrassTex, (IN.worldPos / _GrassScale).xz) * pWeight.y;
				float3 zP = tex2D(_WallTex, (IN.worldPos / _WallScale).xy) * pWeight.z;
				groundColor = (xP + yP + zP);
			}
			else if (height >= _RockStartHeight) {

				float3 xP1 = tex2D(_WallTex, (IN.worldPos / _WallScale).yz) * pWeight.x;
				float3 yP1 = tex2D(_GrassTex, (IN.worldPos / _GrassScale).xz) * pWeight.y;
				float3 zP1 = tex2D(_WallTex, (IN.worldPos / _WallScale).xy) * pWeight.z;
				float3 groundBlend = xP1 + yP1 + zP1;

				float3 xP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).yz) * pWeight.x;
				float3 yP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).xz) * pWeight.y;
				float3 zP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).xy) * pWeight.z;
				float3 rockBlend = xP2 + yP2 + zP2;

				float3 blendedAlpha = groundBlend * ((height - _RockStartHeight) / (_GrassStartHeight - _RockStartHeight)) + rockBlend * ((_GrassStartHeight - height) / (_GrassStartHeight - _RockStartHeight));
				groundColor = blendedAlpha;
			}
			else {
				float3 xP = tex2D(_RockTex, (IN.worldPos / _RockScale).yz) * pWeight.x;
				float3 yP = tex2D(_RockTex, (IN.worldPos / _RockScale).xz) * pWeight.y;
				float3 zP = tex2D(_RockTex, (IN.worldPos / _RockScale).xy) * pWeight.z;
				groundColor = xP + yP + zP;
			}

			//might need to transition
			if (height >= _RockStartHeight) {

				bool TransNeed_R = positionInChunk.x >= _ChunkSize - _TransitionOffset && _RightTransitionNeeded == 1;
				bool TransNeed_F = positionInChunk.y >= _ChunkSize - _TransitionOffset && _FrontTransitionNeeded == 1;
				bool TransNeed_RF = positionInChunk.x >= _ChunkSize - _TransitionOffset && positionInChunk.y >= _ChunkSize - _TransitionOffset && _RightFrontTransitionNeeded == 1;

				bool TrasNeedCorner_RtoF = positionInChunk.x >= _ChunkSize - _TransitionOffset && positionInChunk.y >= _ChunkSize - _TransitionOffset && _RightFrontTransitionNeeded == 0 && _RightTransitionNeeded == 0 && _FrontTransitionNeeded == 1;
				bool TrasNeedCorner_FtoR = positionInChunk.x >= _ChunkSize - _TransitionOffset && positionInChunk.y >= _ChunkSize - _TransitionOffset && _RightFrontTransitionNeeded == 0 && _RightTransitionNeeded == 1 && _FrontTransitionNeeded == 0;

				if (TransNeed_R && TransNeed_F) {

					if (positionInChunk.y < positionInChunk.x) {

						float transitionPower = (positionInChunk.x + _TransitionOffset - _ChunkSize) / _TransitionOffset;


						float3 curPlaceColor = groundColor * (1 - transitionPower);

						float3 transition_rightColor;

						if (height > _GrassStartHeight) {


							float3 xP = tex2D(_RightWallTex, (IN.worldPos / _WallScale).yz) * pWeight.x;
							float3 yP = tex2D(_RightGrassTex, (IN.worldPos / _GrassScale).xz) * pWeight.y;
							float3 zP = tex2D(_RightWallTex, (IN.worldPos / _WallScale).xy) * pWeight.z;
							transition_rightColor = (xP + yP + zP);
						}
						else if (height >= _RockStartHeight) {

							float3 xP1 = tex2D(_RightWallTex, (IN.worldPos / _WallScale).yz) * pWeight.x;
							float3 yP1 = tex2D(_RightGrassTex, (IN.worldPos / _GrassScale).xz) * pWeight.y;
							float3 zP1 = tex2D(_RightWallTex, (IN.worldPos / _WallScale).xy) * pWeight.z;
							float3 groundBlend = xP1 + yP1 + zP1;

							float3 xP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).yz) * pWeight.x;
							float3 yP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).xz) * pWeight.y;
							float3 zP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).xy) * pWeight.z;
							float3 rockBlend = xP2 + yP2 + zP2;

							float3 blendedAlpha = groundBlend * ((height - _RockStartHeight) / (_GrassStartHeight - _RockStartHeight)) + rockBlend * ((_GrassStartHeight - height) / (_GrassStartHeight - _RockStartHeight));
							transition_rightColor = blendedAlpha;
						}

						transition_rightColor = transition_rightColor * transitionPower;





						groundColor = curPlaceColor + transition_rightColor;
					}
					else {

						float transitionPower = (positionInChunk.y + _TransitionOffset - _ChunkSize) / _TransitionOffset;


						float3 curPlaceColor = groundColor * (1 - transitionPower);

						float3 transition_frontColor;

						if (height > _GrassStartHeight) {


							float3 xP = tex2D(_FrontWallTex, (IN.worldPos / _WallScale).yz) * pWeight.x;
							float3 yP = tex2D(_FrontGrassTex, (IN.worldPos / _GrassScale).xz) * pWeight.y;
							float3 zP = tex2D(_FrontWallTex, (IN.worldPos / _WallScale).xy) * pWeight.z;
							transition_frontColor = (xP + yP + zP);
						}
						else if (height >= _RockStartHeight) {

							float3 xP1 = tex2D(_FrontWallTex, (IN.worldPos / _WallScale).yz) * pWeight.x;
							float3 yP1 = tex2D(_FrontGrassTex, (IN.worldPos / _GrassScale).xz) * pWeight.y;
							float3 zP1 = tex2D(_FrontWallTex, (IN.worldPos / _WallScale).xy) * pWeight.z;
							float3 groundBlend = xP1 + yP1 + zP1;

							float3 xP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).yz) * pWeight.x;
							float3 yP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).xz) * pWeight.y;
							float3 zP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).xy) * pWeight.z;
							float3 rockBlend = xP2 + yP2 + zP2;

							float3 blendedAlpha = groundBlend * ((height - _RockStartHeight) / (_GrassStartHeight - _RockStartHeight)) + rockBlend * ((_GrassStartHeight - height) / (_GrassStartHeight - _RockStartHeight));
							transition_frontColor = blendedAlpha;
						}

						transition_frontColor = transition_frontColor * transitionPower;





						groundColor = curPlaceColor + transition_frontColor;
					}


				}
				else if (TrasNeedCorner_RtoF) {
					float transitionPower = (positionInChunk.y - positionInChunk.x) / _TransitionOffset;

					if (transitionPower < 0)
						transitionPower = 0;

					float3 curPlaceColor = groundColor * (1 - transitionPower);

					float3 transition_frontColor;

					if (height > _GrassStartHeight) {


						float3 xP = tex2D(_FrontWallTex, (IN.worldPos / _WallScale).yz) * pWeight.x;
						float3 yP = tex2D(_FrontGrassTex, (IN.worldPos / _GrassScale).xz) * pWeight.y;
						float3 zP = tex2D(_FrontWallTex, (IN.worldPos / _WallScale).xy) * pWeight.z;
						transition_frontColor = (xP + yP + zP);
					}
					else if (height >= _RockStartHeight) {

						float3 xP1 = tex2D(_FrontWallTex, (IN.worldPos / _WallScale).yz) * pWeight.x;
						float3 yP1 = tex2D(_FrontGrassTex, (IN.worldPos / _GrassScale).xz) * pWeight.y;
						float3 zP1 = tex2D(_FrontWallTex, (IN.worldPos / _WallScale).xy) * pWeight.z;
						float3 groundBlend = xP1 + yP1 + zP1;

						float3 xP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).yz) * pWeight.x;
						float3 yP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).xz) * pWeight.y;
						float3 zP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).xy) * pWeight.z;
						float3 rockBlend = xP2 + yP2 + zP2;

						float3 blendedAlpha = groundBlend * ((height - _RockStartHeight) / (_GrassStartHeight - _RockStartHeight)) + rockBlend * ((_GrassStartHeight - height) / (_GrassStartHeight - _RockStartHeight));
						transition_frontColor = blendedAlpha;
					}

					transition_frontColor = transition_frontColor * transitionPower;





					groundColor = curPlaceColor + transition_frontColor;
				}
				else if (TrasNeedCorner_FtoR) {

					if (positionInChunk.y > -positionInChunk.x + 2 * _ChunkSize - _TransitionOffset) {

						float transitionPower = 1 - (positionInChunk.y + _TransitionOffset - _ChunkSize) / _TransitionOffset;


						float3 curPlaceColor = groundColor * (1 - transitionPower);

						float3 transition_rightColor;

						if (height > _GrassStartHeight) {


							float3 xP = tex2D(_RightWallTex, (IN.worldPos / _WallScale).yz) * pWeight.x;
							float3 yP = tex2D(_RightGrassTex, (IN.worldPos / _GrassScale).xz) * pWeight.y;
							float3 zP = tex2D(_RightWallTex, (IN.worldPos / _WallScale).xy) * pWeight.z;
							transition_rightColor = (xP + yP + zP);
						}
						else if (height >= _RockStartHeight) {

							float3 xP1 = tex2D(_RightWallTex, (IN.worldPos / _WallScale).yz) * pWeight.x;
							float3 yP1 = tex2D(_RightGrassTex, (IN.worldPos / _GrassScale).xz) * pWeight.y;
							float3 zP1 = tex2D(_RightWallTex, (IN.worldPos / _WallScale).xy) * pWeight.z;
							float3 groundBlend = xP1 + yP1 + zP1;

							float3 xP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).yz) * pWeight.x;
							float3 yP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).xz) * pWeight.y;
							float3 zP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).xy) * pWeight.z;
							float3 rockBlend = xP2 + yP2 + zP2;

							float3 blendedAlpha = groundBlend * ((height - _RockStartHeight) / (_GrassStartHeight - _RockStartHeight)) + rockBlend * ((_GrassStartHeight - height) / (_GrassStartHeight - _RockStartHeight));
							transition_rightColor = blendedAlpha;
						}

						transition_rightColor = transition_rightColor * transitionPower;





						groundColor = curPlaceColor + transition_rightColor;
					}
					else {

						float transitionPower = (positionInChunk.x + _TransitionOffset - _ChunkSize) / _TransitionOffset;


						float3 curPlaceColor = groundColor * (1 - transitionPower);

						float3 transition_frontColor;

						if (height > _GrassStartHeight) {


							float3 xP = tex2D(_RightWallTex, (IN.worldPos / _WallScale).yz) * pWeight.x;
							float3 yP = tex2D(_RightGrassTex, (IN.worldPos / _GrassScale).xz) * pWeight.y;
							float3 zP = tex2D(_RightWallTex, (IN.worldPos / _WallScale).xy) * pWeight.z;
							transition_frontColor = (xP + yP + zP);
						}
						else if (height >= _RockStartHeight) {

							float3 xP1 = tex2D(_RightWallTex, (IN.worldPos / _WallScale).yz) * pWeight.x;
							float3 yP1 = tex2D(_RightGrassTex, (IN.worldPos / _GrassScale).xz) * pWeight.y;
							float3 zP1 = tex2D(_RightWallTex, (IN.worldPos / _WallScale).xy) * pWeight.z;
							float3 groundBlend = xP1 + yP1 + zP1;

							float3 xP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).yz) * pWeight.x;
							float3 yP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).xz) * pWeight.y;
							float3 zP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).xy) * pWeight.z;
							float3 rockBlend = xP2 + yP2 + zP2;

							float3 blendedAlpha = groundBlend * ((height - _RockStartHeight) / (_GrassStartHeight - _RockStartHeight)) + rockBlend * ((_GrassStartHeight - height) / (_GrassStartHeight - _RockStartHeight));
							transition_frontColor = blendedAlpha;
						}

						transition_frontColor = transition_frontColor * transitionPower;





						groundColor = curPlaceColor + transition_frontColor;
					}

				}
				else if (TransNeed_R) {
					float transitionPower = (positionInChunk.x + _TransitionOffset - _ChunkSize) / _TransitionOffset;


					float3 curPlaceColor = groundColor * (1 - transitionPower);

					float3 transition_rightColor;

					if (height > _GrassStartHeight) {


						float3 xP = tex2D(_RightWallTex, (IN.worldPos / _WallScale).yz) * pWeight.x;
						float3 yP = tex2D(_RightGrassTex, (IN.worldPos / _GrassScale).xz) * pWeight.y;
						float3 zP = tex2D(_RightWallTex, (IN.worldPos / _WallScale).xy) * pWeight.z;
						transition_rightColor = (xP + yP + zP);
					}
					else if (height >= _RockStartHeight) {

						float3 xP1 = tex2D(_RightWallTex, (IN.worldPos / _WallScale).yz) * pWeight.x;
						float3 yP1 = tex2D(_RightGrassTex, (IN.worldPos / _GrassScale).xz) * pWeight.y;
						float3 zP1 = tex2D(_RightWallTex, (IN.worldPos / _WallScale).xy) * pWeight.z;
						float3 groundBlend = xP1 + yP1 + zP1;

						float3 xP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).yz) * pWeight.x;
						float3 yP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).xz) * pWeight.y;
						float3 zP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).xy) * pWeight.z;
						float3 rockBlend = xP2 + yP2 + zP2;

						float3 blendedAlpha = groundBlend * ((height - _RockStartHeight) / (_GrassStartHeight - _RockStartHeight)) + rockBlend * ((_GrassStartHeight - height) / (_GrassStartHeight - _RockStartHeight));
						transition_rightColor = blendedAlpha;
					}

					transition_rightColor = transition_rightColor *  transitionPower;





					groundColor = curPlaceColor + transition_rightColor;
				}
				else if (TransNeed_F) {
					float transitionPower = (positionInChunk.y + _TransitionOffset - _ChunkSize) / _TransitionOffset;


					float3 curPlaceColor = groundColor * (1 - transitionPower);

					float3 transition_frontColor;

					if (height > _GrassStartHeight) {


						float3 xP = tex2D(_FrontWallTex, (IN.worldPos / _WallScale).yz) * pWeight.x;
						float3 yP = tex2D(_FrontGrassTex, (IN.worldPos / _GrassScale).xz) * pWeight.y;
						float3 zP = tex2D(_RightWallTex, (IN.worldPos / _WallScale).xy) * pWeight.z;
						transition_frontColor = (xP + yP + zP);
					}
					else if (height >= _RockStartHeight) {

						float3 xP1 = tex2D(_FrontWallTex, (IN.worldPos / _WallScale).yz) * pWeight.x;
						float3 yP1 = tex2D(_FrontGrassTex, (IN.worldPos / _GrassScale).xz) * pWeight.y;
						float3 zP1 = tex2D(_FrontWallTex, (IN.worldPos / _WallScale).xy) * pWeight.z;
						float3 groundBlend = xP1 + yP1 + zP1;

						float3 xP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).yz) * pWeight.x;
						float3 yP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).xz) * pWeight.y;
						float3 zP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).xy) * pWeight.z;
						float3 rockBlend = xP2 + yP2 + zP2;

						float3 blendedAlpha = groundBlend * ((height - _RockStartHeight) / (_GrassStartHeight - _RockStartHeight)) + rockBlend * ((_GrassStartHeight - height) / (_GrassStartHeight - _RockStartHeight));
						transition_frontColor = blendedAlpha;
					}

					transition_frontColor = transition_frontColor * transitionPower;





					groundColor = curPlaceColor + transition_frontColor;
				}
				else if (TransNeed_RF) {

					float transitionPower = (positionInChunk.y + positionInChunk.x - 2 * _ChunkSize + _TransitionOffset) / _TransitionOffset;

					if (transitionPower < 0)
						transitionPower = 0;

					float3 curPlaceColor = groundColor * (1 - transitionPower);

					float3 transition_frontColor;

					if (height > _GrassStartHeight) {


						float3 xP = tex2D(_RightFrontWallTex, (IN.worldPos / _WallScale).yz) * pWeight.x;
						float3 yP = tex2D(_RightFrontGrassTex, (IN.worldPos / _GrassScale).xz) * pWeight.y;
						float3 zP = tex2D(_RightFrontWallTex, (IN.worldPos / _WallScale).xy) * pWeight.z;
						transition_frontColor = (xP + yP + zP);
					}
					else if (height >= _RockStartHeight) {

						float3 xP1 = tex2D(_RightFrontWallTex, (IN.worldPos / _WallScale).yz) * pWeight.x;
						float3 yP1 = tex2D(_RightFrontGrassTex, (IN.worldPos / _GrassScale).xz) * pWeight.y;
						float3 zP1 = tex2D(_RightFrontWallTex, (IN.worldPos / _WallScale).xy) * pWeight.z;
						float3 groundBlend = xP1 + yP1 + zP1;

						float3 xP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).yz) * pWeight.x;
						float3 yP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).xz) * pWeight.y;
						float3 zP2 = tex2D(_RockTex, (IN.worldPos / _RockScale).xy) * pWeight.z;
						float3 rockBlend = xP2 + yP2 + zP2;

						float3 blendedAlpha = groundBlend * ((height - _RockStartHeight) / (_GrassStartHeight - _RockStartHeight)) + rockBlend * ((_GrassStartHeight - height) / (_GrassStartHeight - _RockStartHeight));
						transition_frontColor = blendedAlpha;
					}

					transition_frontColor = transition_frontColor * transitionPower;





					groundColor = curPlaceColor + transition_frontColor;

				}

			}



			
			o.Albedo = groundColor;

		}

		ENDCG
			
	}
		Fallback "Diffuse"
}
