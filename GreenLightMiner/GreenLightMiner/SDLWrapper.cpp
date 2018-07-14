#include "sdlwrapper.h"

#include <boost/range/algorithm/for_each.hpp>

#include <stdexcept>
#include <iostream>

using namespace glm;

SDLWrapper::SDLWrapper()
{
	if (SDL_Init(SDL_INIT_VIDEO) < 0)
	{
		throw std::runtime_error(std::string("SDL could not initalize! SDL Error ") + SDL_GetError());
	}

	if (SDL_CreateWindowAndRenderer(SCREEN_WIDTH, SCREEN_HEIGHT, 0, &m_pWindow, &m_pRenderer) != 0)
	{
		throw std::runtime_error(std::string("Window could not be created! SDL Error: ") + SDL_GetError());
	}
}

SDLWrapper::~SDLWrapper()
{
	if (m_pRenderer)
	{
		SDL_DestroyRenderer(m_pRenderer);
	}

	if (m_pWindow)
	{
		SDL_DestroyWindow(m_pWindow);
	}

	SDL_Quit();
}

void SDLWrapper::Draw(const Point2iVector & points) const
{
	SDL_bool done = SDL_FALSE;

	while (!done) {
		SDL_Event event;

		SDL_SetRenderDrawColor(m_pRenderer, 0, 0, 0, SDL_ALPHA_OPAQUE);
		SDL_RenderClear(m_pRenderer);

		SDL_SetRenderDrawColor(m_pRenderer, 0xFF, 0xFF, 0xFF, SDL_ALPHA_OPAQUE);
		
		boost::for_each(points,
			[&](const auto & p)
		{
			SDL_RenderDrawPoint(m_pRenderer, p.x, p.y);
		});
		
		SDL_RenderPresent(m_pRenderer);

		while (SDL_PollEvent(&event)) {
			if (event.type == SDL_QUIT) {
				done = SDL_TRUE;
			}
		}
	}
}
