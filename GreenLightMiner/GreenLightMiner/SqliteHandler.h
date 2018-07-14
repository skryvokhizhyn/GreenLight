#pragma once

#include "DBQueryResult.h"

#include <sqlite3.h>

#include <string>

namespace glm
{
	class SqliteHandler
	{
	public:
		SqliteHandler(const std::string & path);
		~SqliteHandler();

		DBQueryResult Execute(const std::string & query);

	public:
		SqliteHandler(const SqliteHandler&) = delete;
		SqliteHandler(SqliteHandler&&) = delete;

		SqliteHandler& operator= (const SqliteHandler&) = delete;

	private:
		sqlite3 * m_pDb = nullptr;
	};
}
