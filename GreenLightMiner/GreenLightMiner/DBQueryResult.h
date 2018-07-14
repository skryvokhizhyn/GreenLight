#pragma once

#include <sqlite3.h>

#include <gsl.h>

namespace glm
{
	class DBQueryResult
	{
	public:
		DBQueryResult(gsl::not_null<sqlite3*> dbConnection, const std::string & query);
		~DBQueryResult();

		bool Next();

		template<typename T>
		T Get(uint16_t) const;

	private:
		sqlite3 * m_pDb = nullptr;
		sqlite3_stmt * m_pStmt = nullptr;
	};
}