#pragma once

#include "Point2.h"

#include <SDL.h>

namespace glm
{
	class SDLWrapper
	{
	public:
		static const int SCREEN_WIDTH = 800;
		static const int SCREEN_HEIGHT = 800;

	public:
		SDLWrapper();
		~SDLWrapper();

		void Draw(const Point2iVector & points) const;

	private:
		SDL_Window * m_pWindow = nullptr;
		SDL_Renderer * m_pRenderer = nullptr;
	};
}
