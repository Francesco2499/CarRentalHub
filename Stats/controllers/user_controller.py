from flask import Blueprint, request
from services.user_service import (
    generate_top_users_plot,
    generate_users_by_region_plot,
    get_user_info
)

user_stats_blueprint = Blueprint("user_stats", __name__)

@user_stats_blueprint.route('/top3', methods=['GET'])
def user_top3():
    return generate_top_users_plot()

@user_stats_blueprint.route('/regions', methods=['GET'])
def user_regions_distribution():
    return generate_users_by_region_plot()

@user_stats_blueprint.route('/info/<int:user_id>', methods=['GET'])
def user_info(user_id):
    return get_user_info(user_id)
