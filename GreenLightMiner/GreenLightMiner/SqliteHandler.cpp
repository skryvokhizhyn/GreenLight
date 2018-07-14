#include "SqliteHandler.h"

#include <sstream>

using namespace glm;

SqliteHandler::SqliteHandler(const std::string & path)
{
	const int rc = sqlite3_open_v2(path.c_str(), &m_pDb, SQLITE_OPEN_READWRITE, nullptr);
	if (rc) 
	{
		std::stringstream errMsg;
		errMsg << "Can't open database: " << sqlite3_errmsg(m_pDb);
		
		sqlite3_close(m_pDb);

		throw std::runtime_error(errMsg.str());
	}
}

SqliteHandler::~SqliteHandler()
{
	sqlite3_close(m_pDb);
}

DBQueryResult SqliteHandler::Execute(const std::string & query)
{
	return DBQueryResult(m_pDb, query);
}
